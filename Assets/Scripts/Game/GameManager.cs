using System;
using System.Collections;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;
using UnityEngine.Android;

namespace KitchenKrapper
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private PlatformType currentPlatform = PlatformManager.CurrentPlatform;
        private GameStatus gameStatus = GameStatus.None;

        public LevelSO currentLevel;

        public static event Action PlayerDataChanged;
        public static event Action GameDataUpdated;

        [SerializeField] private PlayerGameData playerGameData;
        [SerializeField] private GameData gameData;

        public PlayerGameData PlayerGameData
        {
            get { return playerGameData; }
            set { playerGameData = value; }
        }

        public GameData GameData
        {
            get { return gameData; }
            set { gameData = value; }
        }

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
        }

        private void Start()
        {
            AudioManager.Instance.AudioSettingsChanged += OnGameDataUpdated;
            PlayerNamePopupScreen.PlayerNameSet += CreatePlayerData;
            PlayerDataStorage.PlayerDataCreated += StartGame;
            EULAScreen.EULAAccepted += OnEULAAccepted;

            if (currentPlatform == PlatformType.Android)
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                {
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                }
            }

            InitializeApplication();
        }

        private void OnDestroy()
        {
            AudioManager.Instance.AudioSettingsChanged -= OnGameDataUpdated;
            PlayerNamePopupScreen.PlayerNameSet -= CreatePlayerData;
            PlayerDataStorage.PlayerDataCreated -= StartGame;
            EULAScreen.EULAAccepted -= OnEULAAccepted;
        }

        public void InitializeApplication()
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

        public void ShowEULAScreen()
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
            gameData.EulaAgreed = true;
            GameDataUpdated?.Invoke();
            InitializeApplication();
        }

        private void OnGameDataUpdated()
        {
            GameDataUpdated?.Invoke();
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
                PlayerGameData = JsonUtility.FromJson<PlayerGameData>(playerData);
                print("[GameManager]: Player Data: " + playerData);
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
            PlayerDataStorage.Instance.SetPlayerData(playerGameData.ToJson());
        }

        public void SetCoins(uint coins)
        {
            playerGameData.Coins = coins;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void SetGems(uint gems)
        {
            playerGameData.Gems = gems;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void SetPlayerDisplayName(string playerDisplayName)
        {
            playerGameData.PlayerDisplayName = playerDisplayName;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void SetPlayerIcon(string playerIcon)
        {
            playerGameData.PlayerIcon = playerIcon;
            UpdatePlayerData();
            PlayerDataChanged?.Invoke();
        }

        public void SetPlayerTrophies(uint playerTrophies)
        {
            playerGameData.PlayerTrophies = playerTrophies;
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
