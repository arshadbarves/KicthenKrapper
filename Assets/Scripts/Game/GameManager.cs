using System;
using System.Collections;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;

namespace KitchenKrapper
{
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }

        // Current game status
        private GameStatus gameStatus = GameStatus.None;
        public static event Action<GameData> FundsUpdated;
        public static event Action PlayerDataChanged;

        // Getter and setter for game status
        public GameStatus GameStatus
        {
            get { return gameStatus; }
            set { gameStatus = value; }
        }

        [SerializeField] private GameData gameData;
        public GameData GameData
        {
            get { return gameData; }
            set { gameData = value; }
        }

        // Initialize the singleton instance
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            gameData = new GameData();
        }

        // Entry point for the game
        private void Start()
        {
            InitializeApplication();
        }

        // Initialize the application
        public void InitializeApplication()
        {
            MainMenuUIManager.Instance.ShowLoginScreen();

            if (!ClientPrefs.GetEULAAndPrivacyPolicyAccepted())
            {
                Invoke("ShowEULAScreen", 0.5f);
            }
            else
            {
                StartApp();
            }
        }

        // Show EULA screen
        public void ShowEULAScreen()
        {
            MainMenuUIManager.Instance.ShowEULAScreen();
        }

        // Start the application
        public void StartApp()
        {
            StartCoroutine(CheckInternetConnection());
            EOSAuth.Instance.Login();
            ClientPrefs.Initialize();
            GameDataSource.Instance.Initialize();
        }

        // Start the game
        public void StartGame()
        {
            gameStatus = GameStatus.GameStarted;
            // SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString());
            MainMenuUIManager.Instance.ShowHomeScreen();
            PlayerDataStorage.Instance.GetPlayerData(PlayerDataStorageCallback);
        }

        // Quit the game
        public void QuitGame()
        {
            gameStatus = GameStatus.GameQuit;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // Coroutine to check internet connection
        private IEnumerator CheckInternetConnection()
        {
            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    MainMenuUIManager.Instance.ShowNetworkDisconnectedScreen();
                }
                else
                {
                    MainMenuUIManager.Instance.HideNetworkDisconnectedScreen();
                }

                yield return new WaitForSeconds(1.0f);
            }
        }

        // Reset player authentication
        private void ResetPlayerAuth()
        {
            var authInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
            var options = new DeletePersistentAuthOptions();

            authInterface.DeletePersistentAuth(ref options, null, (ref DeletePersistentAuthCallbackInfo deletePersistentAuthCallbackInfo) =>
            {
                if (deletePersistentAuthCallbackInfo.ResultCode == Result.Success)
                {
                    Debug.Log("Persistent auth deleted");
                }
                else
                {
                    Debug.Log("Persistent auth not deleted");
                }
            });

            var connectInterface = EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
            var connectOptions = new Epic.OnlineServices.Connect.DeleteDeviceIdOptions();

            connectInterface.DeleteDeviceId(ref connectOptions, null, (ref DeleteDeviceIdCallbackInfo deleteDeviceIdCallbackInfo) =>
            {
                if (deleteDeviceIdCallbackInfo.ResultCode == Result.Success)
                {
                    Debug.Log("Device ID deleted");
                }
                else
                {
                    Debug.Log("Device ID not deleted");
                }
            });
        }

        // Cleanup application and reset data
        public void CleanupApplication()
        {
            GameDataSource.Instance.ResetGameData();
            gameStatus = GameStatus.None;
            ClientPrefs.ResetClientPrefs();
            ResetPlayerAuth();
            InitializeApplication();

            if (ApplicationManager.Instance != null)
            {
                Destroy(ApplicationManager.Instance.gameObject);
            }
        }

        // Callback for player data storage
        public void PlayerDataStorageCallback(string playerData)
        {
            if (!string.IsNullOrEmpty(playerData))
            {
                gameData = JsonUtility.FromJson<GameData>(playerData);
                PlayerDataChanged?.Invoke();
            }
            else
            {
                MainMenuUIManager.Instance.ShowPlayerNamePopupScreen();
            }
        }

        // Create player data
        public void CreatePlayerData(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                return;
            }

            GameData playerData = new GameData
            {
                PlayerDisplayName = playerName,
                PlayerId = EOSManager.Instance.GetProductUserId()
            };

            Debug.Log("[GameManager]: Creating Player Account: " + playerData.PlayerId + " " + playerData.PlayerDisplayName);

            PlayerDataStorage.Instance.CreatePlayerData(playerData);
            PlayerDataStorage.Instance.GetPlayerData(PlayerDataStorageCallback);
        }

        // Update player data
        public void UpdatePlayerData()
        {
            PlayerDataStorage.Instance.SetPlayerData(gameData.ToJson());
        }

        // Set coins
        public void SetCoins(uint coins)
        {
            gameData.Coins = coins;
            UpdatePlayerData();
            FundsUpdated?.Invoke(gameData);
        }

        // Set gems
        public void SetGems(uint gems)
        {
            gameData.Gems = gems;
            UpdatePlayerData();
            FundsUpdated?.Invoke(gameData);
        }

        // Set player display name
        public void SetPlayerDisplayName(string playerDisplayName)
        {
            gameData.PlayerDisplayName = playerDisplayName;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        // Set player icon
        public void SetPlayerIcon(string playerIcon)
        {
            gameData.PlayerIcon = playerIcon;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        // Set player trophies
        public void SetPlayerTrophies(uint playerTrophies)
        {
            gameData.PlayerTrophies = playerTrophies;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        // Logout
        public void Logout()
        {
            EOSAuth.Instance.Logout();
            CleanupApplication();
        }
    }
}
