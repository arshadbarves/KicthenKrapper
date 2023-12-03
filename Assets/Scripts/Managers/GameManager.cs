using System;
using System.Collections;
using KitchenKrapper;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;
using UnityEngine.Android;
using Utils.Enums;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private static PlatformType CurrentPlatform => PlatformManager.CurrentPlatform;
        private GameStatus _gameStatus = GameStatus.None;
        private InputType _gameInputType;

        public LevelSO currentLevel;
        public PlayerGameData PlayerData
        {
            get => playerData;
            private set
            {
                if (playerData == value) return;
                playerData = value;
                PlayerDataChanged?.Invoke();
            }
        }
        public GameData GameData
        {
            get => gameData;
            set
            {
                if (gameData == value) return;
                gameData = value;
                GameDataUpdated?.Invoke();
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
            switch (CurrentPlatform)
            {
                case PlatformType.Android:
                    if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) &&
                        Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)) return;
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                    _gameInputType = InputType.Touch;
                    break;
                case PlatformType.iOS:
                    break;
                case PlatformType.Windows:
                    _gameInputType = InputType.Keyboard;
                    break;
                case PlatformType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public InputType GetInputType()
        {
            return _gameInputType;
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
                Invoke(nameof(ShowEulaScreen), 0.5f);
            }
        }

        private void ShowEulaScreen()
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
            _gameStatus = GameStatus.GameStarted;
            PlayerDataStorage.Instance.GetPlayerData(PlayerDataStorageCallback);
        }

        public void QuitGame()
        {
            _gameStatus = GameStatus.GameQuit;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnEULAAccepted()
        {
            GameData data = new GameData
            {
                EulaAgreed = true
            };
            GameData = data;
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
            // ReSharper disable once IteratorNeverReturns
        }

        private void PlayerDataStorageCallback(string newPlayerData)
        {
            if (!string.IsNullOrEmpty(newPlayerData))
            {
                PlayerData = JsonUtility.FromJson<PlayerGameData>(newPlayerData);
                Debug.Log("[GameManager]: Player Data: " + newPlayerData);
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

            var playerGameData = new PlayerGameData
            {
                PlayerDisplayName = playerName,
                PlayerId = EOSManager.Instance.GetProductUserId()
            };

            Debug.Log("[GameManager]: Creating Player Account: " + playerGameData.PlayerId + " " + playerGameData.PlayerDisplayName);

            PlayerDataStorage.Instance.CreatePlayerData(playerGameData);
        }

        private void UpdatePlayerData()
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
            _gameStatus = GameStatus.None;
            SaveManager.Instance.ResetGame();
            InitializeApplication();
        }

        public LevelSO GetCurrentLevel()
        {
            return currentLevel;
        }
    }
}
