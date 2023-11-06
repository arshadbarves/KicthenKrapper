using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using PlayEveryWare.EpicOnlineServices;

namespace KitchenKrapper
{
    public class GameManager : Singleton<GameManager>
    {
        private PlatformType currentPlatform => PlatformManager.CurrentPlatform;
        private GameStatus gameStatus = GameStatus.None;

        public LevelSO currentLevel;
        public PlayerGameData PlayerData
        {
            get { return playerData; }
            set
            {
                if (playerData != value)
                {
                    playerData = value;
                    PlayerDataChanged?.Invoke();
                }
            }
        }
        public GameData GameData
        {
            get { return gameData; }
            set
            {
                if (gameData != value)
                {
                    gameData = value;
                    GameDataUpdated?.Invoke();
                }
            }
        }

        public static event Action PlayerDataChanged;
        public static event Action GameDataUpdated;

        [SerializeField] private PlayerGameData playerData;
        [SerializeField] private GameData gameData;

        private void Start()
        {
            SetupEventListeners();
            RequestStoragePermissionsIfNeeded();
            InitializeApplication();
        }

        protected override void OnDestroy()
        {
            RemoveEventListeners();
        }

        private void SetupEventListeners()
        {
            PlayerNamePopupScreen.PlayerNameSet += CreatePlayerData;
            PlayerDataStorage.PlayerDataCreated += StartGame;
            EULAScreen.EULAAccepted += OnEULAAccepted;
        }

        private void RemoveEventListeners()
        {
            PlayerNamePopupScreen.PlayerNameSet -= CreatePlayerData;
            PlayerDataStorage.PlayerDataCreated -= StartGame;
            EULAScreen.EULAAccepted -= OnEULAAccepted;
        }

        private void RequestStoragePermissionsIfNeeded()
        {
            if (currentPlatform == PlatformType.Android)
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) ||
                    !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                {
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                }
            }
        }

        private void InitializeApplication()
        {
            SaveManager.Instance.LoadGame();
            MainMenuUIManager.Instance.ShowLoginScreen();

            if (gameData.EulaAgreed)
            {
                StartApp();
            }
            else
            {
                Invoke(nameof(ShowEULAScreen), 0.5f);
            }
        }

        private void ShowEULAScreen()
        {
            MainMenuUIManager.Instance.ShowEULAScreen();
        }

        public void StartApp()
        {
            StartCoroutine(CheckInternetConnection());
            EOSAuth.Instance.Login();
        }

        public void StartGame()
        {
            gameStatus = GameStatus.GameStarted;
            PlayerDataStorage.Instance.GetPlayerData(PlayerDataStorageCallback);
        }

        public void QuitGame()
        {
            gameStatus = GameStatus.GameQuit;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnEULAAccepted()
        {
            GameData.EulaAgreed = true;
            InitializeApplication();
        }

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

        public void PlayerDataStorageCallback(string playerData)
        {
            if (!string.IsNullOrEmpty(playerData))
            {
                PlayerData = JsonUtility.FromJson<PlayerGameData>(playerData);
                Debug.Log("[GameManager]: Player Data: " + playerData);
                MainMenuUIManager.Instance.ShowHomeScreen();
                PlayerDataChanged?.Invoke();
            }
            else
            {
                MainMenuUIManager.Instance.ShowPlayerNamePopupScreen();
            }
        }

        public void CreatePlayerData(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                return;
            }

            PlayerGameData playerData = new PlayerGameData
            {
                PlayerDisplayName = playerName,
                PlayerId = EOSManager.Instance.GetProductUserId()
            };

            Debug.Log("[GameManager]: Creating Player Account: " + playerData.PlayerId + " " + playerData.PlayerDisplayName);

            PlayerDataStorage.Instance.CreatePlayerData(playerData);
        }

        public void UpdatePlayerData()
        {
            PlayerDataStorage.Instance.SetPlayerData(playerData.ToJson());
        }

        public string GetPlayerName()
        {
            return playerData.PlayerDisplayName;
        }

        public void SetCoins(uint coins)
        {
            playerData.Coins = coins;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void SetGems(uint gems)
        {
            playerData.Gems = gems;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void SetPlayerDisplayName(string playerDisplayName)
        {
            playerData.PlayerDisplayName = playerDisplayName;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void SetPlayerIcon(string playerIcon)
        {
            playerData.PlayerIcon = playerIcon;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void SetPlayerTrophies(uint playerTrophies)
        {
            playerData.PlayerTrophies = playerTrophies;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void Logout()
        {
            EOSAuth.Instance.Logout();
            gameStatus = GameStatus.None;
            SaveManager.Instance.ResetGame();
            InitializeApplication();
        }

        public LevelSO GetCurrentLevel()
        {
            return currentLevel;
        }
    }
}
