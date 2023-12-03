using Managers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KitchenKrapper
{
    public class SceneLoaderWrapper : NetworkBehaviour
    {
        [SerializeField] private MainMenuUIManager mainMenuUIManager;
        [SerializeField] private LoadingProgressManager loadingProgressManager;

        private bool IsNetworkSceneManagementEnabled => NetworkManager != null && NetworkManager.SceneManager != null && NetworkManager.NetworkConfig.EnableSceneManagement;

        public static SceneLoaderWrapper Instance { get; protected set; }

        private void Awake()
        {
            InitializeSingleton();
            DontDestroyOnLoad(this);
        }

        private void InitializeSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            base.OnDestroy();
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager != null && NetworkManager.SceneManager != null)
            {
                NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
            }
        }

        public virtual void AddOnSceneEventCallback()
        {
            if (IsNetworkSceneManagementEnabled)
            {
                NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
            }
        }

        public virtual void LoadScene(string sceneName, bool useNetworkSceneManager = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (useNetworkSceneManager && IsNetworkSceneManagementEnabled)
            {
                NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
            }
            else
            {
                var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                if (loadSceneMode == LoadSceneMode.Single)
                {
                    mainMenuUIManager.ShowLoadingScreen();
                    loadingProgressManager.LocalLoadOperation = loadOperation;
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!IsSpawned || NetworkManager.ShutdownInProgress)
            {
                mainMenuUIManager.HideLoadingScreen();
            }
        }

        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load:
                    if (NetworkManager.IsClient)
                    {
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
                case SceneEventType.LoadEventCompleted:
                    if (NetworkManager.IsClient)
                    {
                        mainMenuUIManager.HideLoadingScreen();
                        loadingProgressManager.ResetLocalProgress();
                    }
                    break;
                case SceneEventType.Synchronize:
                    if (NetworkManager.IsClient && !NetworkManager.IsHost)
                    {
                        UnloadAdditiveScenes();
                    }
                    break;
                case SceneEventType.SynchronizeComplete:
                    if (NetworkManager.IsServer)
                    {
                        HideLoadingScreenClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { sceneEvent.ClientId } } });
                    }
                    break;
            }
        }

        private void UnloadAdditiveScenes()
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
        private void HideLoadingScreenClientRpc(ClientRpcParams clientRpcParams = default)
        {
            mainMenuUIManager.HideLoadingScreen();
        }

        public string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public void ReloadScene()
        {
            LoadScene(GetActiveSceneName());
        }
    }
}
