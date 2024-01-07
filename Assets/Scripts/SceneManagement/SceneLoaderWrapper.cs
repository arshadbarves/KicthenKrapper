using KitchenKrapper;
using Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    /// <summary>
    /// Manages a loading screen by wrapping around scene management APIs. It loads scene using the SceneManager,
    /// or, on listening servers for which scene management is enabled, using the NetworkSceneManager and handles
    /// the starting and stopping of the loading screen.
    /// </summary>
    public class SceneLoaderWrapper : NetworkSingleton<SceneLoaderWrapper>
    {
        [SerializeField] private MainMenuUIManager mainMenuUIManager;
        [SerializeField] private LoadingProgressManager loadingProgressManager;

        private bool IsNetworkSceneManagementEnabled => NetworkManager != null && NetworkManager.SceneManager != null && NetworkManager.NetworkConfig.EnableSceneManagement;

        private bool _isInitialized;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            NetworkManager.OnServerStarted += OnNetworkingSessionStarted;
            NetworkManager.OnClientStarted += OnNetworkingSessionStarted;
            NetworkManager.OnServerStopped += OnNetworkingSessionEnded;
            NetworkManager.OnClientStopped += OnNetworkingSessionEnded;
        }

        private void OnNetworkingSessionStarted()
        {
            // This prevents this to be called twice on a host, which receives both OnServerStarted and OnClientStarted callbacks
            if (_isInitialized) return;
            if (IsNetworkSceneManagementEnabled)
            {
                NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
            }

            _isInitialized = true;
        }
        
        private void OnNetworkingSessionEnded(bool obj)
        {
            if (!_isInitialized) return;
            if (IsNetworkSceneManagementEnabled)
            {
                NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
            }

            _isInitialized = false;
        }

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (NetworkManager != null)
            {
                NetworkManager.OnServerStarted -= OnNetworkingSessionStarted;
                NetworkManager.OnClientStarted -= OnNetworkingSessionStarted;
                NetworkManager.OnServerStopped -= OnNetworkingSessionEnded;
                NetworkManager.OnClientStopped -= OnNetworkingSessionEnded;
            }
            base.OnDestroy();
        }

        /// <summary>
        /// Loads a scene asynchronously using the specified loadSceneMode, with NetworkSceneManager if on a listening
        /// server with SceneManagement enabled, or SceneManager otherwise. If a scene is loaded via SceneManager, this
        /// method also triggers the start of the loading screen.
        /// </summary>
        /// <param name="sceneName">Name or path of the Scene to load.</param>
        /// <param name="useNetworkSceneManager">If true, uses NetworkSceneManager, else uses SceneManager</param>
        /// <param name="loadSceneMode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
        public void LoadScene(string sceneName, bool useNetworkSceneManager=false, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (useNetworkSceneManager)
            {
                if (!IsSpawned || !IsNetworkSceneManagementEnabled || NetworkManager.ShutdownInProgress) return;
                if (NetworkManager.IsServer)
                {
                    // If is active server and NetworkManager uses scene management, load scene using NetworkManager's SceneManager
                    NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
                }
            }
            else
            {
                // Load using SceneManager
                var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                if (loadSceneMode != LoadSceneMode.Single) return;
                mainMenuUIManager.ShowLoadingScreen();
                loadingProgressManager.LocalLoadOperation = loadOperation;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!IsSpawned || NetworkManager.ShutdownInProgress)
            {
                mainMenuUIManager.StopLoadingScreen();
            }
        }

        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load: // Server told client to load a scene
                    // Only executes on client or host
                    if (NetworkManager.IsClient)
                    {
                        // Only start a new loading screen if scene loaded in Single mode, else simply update
                        if (sceneEvent.LoadSceneMode == LoadSceneMode.Single)
                        {
                            mainMenuUIManager.ShowLoadingScreen();
                            loadingProgressManager.LocalLoadOperation = sceneEvent.AsyncOperation;
                        }
                        else
                        {
                            mainMenuUIManager.UpdateLoadingScreen();
                            loadingProgressManager.LocalLoadOperation = sceneEvent.AsyncOperation;
                        }
                    }
                    break;
                case SceneEventType.LoadEventCompleted: // Server told client that all clients finished loading a scene
                    // Only executes on client or host
                    if (NetworkManager.IsClient)
                    {
                        mainMenuUIManager.StopLoadingScreen();
                    }
                    break;
                case SceneEventType.Synchronize: // Server told client to start synchronizing scenes
                {
                    // Only executes on client that is not the host
                    if (NetworkManager.IsClient && !NetworkManager.IsHost)
                    {
                        if (NetworkManager.SceneManager.ClientSynchronizationMode == LoadSceneMode.Single)
                        {
                            // If using the Single ClientSynchronizationMode, unload all currently loaded additive
                            // scenes. In this case, we want the client to only keep the same scenes loaded as the
                            // server. Netcode For GameObjects will automatically handle loading all the scenes that the
                            // server has loaded to the client during the synchronization process. If the server's main
                            // scene is different to the client's, it will start by loading that scene in single mode,
                            // unloading every additively loaded scene in the process. However, if the server's main
                            // scene is the same as the client's, it will not automatically unload additive scenes, so
                            // we do it manually here.
                            UnloadAdditiveScenes();
                        }
                    }
                    break;
                }
                case SceneEventType.SynchronizeComplete: // Client told server that they finished synchronizing
                    // Only executes on server
                    if (NetworkManager.IsServer)
                    {
                        // Send client RPC to make sure the client stops the loading screen after the server handles what it needs to after the client finished synchronizing, for example character spawning done server side should still be hidden by loading screen.
                        StopLoadingScreenClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { sceneEvent.ClientId } } });
                    }
                    break;
            }
        }

        private static void UnloadAdditiveScenes()
        {
            var activeScene = SceneManager.GetActiveScene();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene != activeScene)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }
        
        [ClientRpc]
        private void StopLoadingScreenClientRpc(ClientRpcParams clientRpcParams = default)
        {
            mainMenuUIManager.StopLoadingScreen();
        }

        private static string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public void ReloadScene()
        {
            LoadScene(GetActiveSceneName());
        }
    }
}
