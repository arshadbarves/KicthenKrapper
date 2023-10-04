using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuUIManager : MonoBehaviour
    {
        [Header("Modal Menu Screens")]
        [Tooltip("Only one modal interface can appear on-screen at a time.")]
        [SerializeField] HomeScreen homeModalScreen;
        [SerializeField] SettingsScreen settingsModalScreen;
        [SerializeField] ClientLoadingScreen clientLoadingScreen;
        [SerializeField] LoginScreen loginModalScreen;
        [SerializeField] MainMenuScreen mainMenuScreen;

        [Header("Overlay Screens")]
        [Tooltip("Overlay screens can appear on-screen at the same time as a modal interface.")]
        [SerializeField] NetworkDisconnectedScreen networkDisconnectedScreen;
        [SerializeField] BufferScreen bufferScreen;

        [Header("Pop-up Screens")]
        [Tooltip("Pop-up screens can appear on-screen at the same time as a modal interface.")]
        [SerializeField] EULAScreen eulaModalScreen;
        [SerializeField] PlayerNamePopupScreen playerNamePopupScreen;

        [Header("Toolbars")]
        [Tooltip("Toolbars remain active at all times unless explicitly disabled.")]
        [SerializeField] OptionbarScreen optionbarScreen;

        public const string PopupPanelActiveClassName = "popup-panel";
        public const string PopupPanelInactiveClassName = "popup-panel--inactive";
        public const string ModalPanelActiveClassName = "modal-panel";
        public const string ModalPanelInactiveClassName = "modal-panel--inactive";

        List<Screen> allModalScreens = new List<Screen>();

        UIDocument mainMenuDocument;

        public UIDocument MainMenuDocument => mainMenuDocument;

        public static MainMenuUIManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton pattern & don't destroy on load
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
        }

        void OnEnable()
        {
            mainMenuDocument = GetComponent<UIDocument>();
            SetupModalScreens();
        }

        private void ShowAllModalScreens()
        {
            foreach (Screen m in allModalScreens)
            {
                m.ShowScreen();
            }
        }

        private void HideAllModalScreens(bool hideAll = true)
        {
            foreach (Screen m in allModalScreens)
            {
                if (hideAll)
                {
                    print("Hiding all modal screens" + m);
                    m.HideScreen();
                }
                else if (m.GetScreenType() == ScreenType.Modal)
                    m.HideScreen();
            }
        }

        void SetupModalScreens()
        {
            allModalScreens?.Add(homeModalScreen);
            allModalScreens?.Add(settingsModalScreen);
            allModalScreens?.Add(clientLoadingScreen);
            allModalScreens?.Add(loginModalScreen);
            allModalScreens?.Add(mainMenuScreen);
            allModalScreens?.Add(eulaModalScreen);
            allModalScreens?.Add(networkDisconnectedScreen);
            allModalScreens?.Add(bufferScreen);
            allModalScreens?.Add(playerNamePopupScreen);

            // if (charModalScreen != null)
            //     allModalScreens.Add(charModalScreen);

            ShowAllModalScreens();
            HideAllModalScreens();
        }

        private void HideMenuModalScreens()
        {
            homeModalScreen?.HideScreen();
            settingsModalScreen?.HideScreen();
        }

        void ShowModalScreen(Screen modalScreen, bool hideAll = true)
        {
            HideAllModalScreens(hideAll);
            modalScreen?.ShowScreen();
        }

        public void ShowHomeScreen()
        {
            ShowMainMenuScreen();
            HideMenuModalScreens();
            homeModalScreen?.ShowScreen();
        }

        public void ShowInventoryScreen()
        {
            ShowMainMenuScreen();
            HideMenuModalScreens();
            homeModalScreen?.ShowScreen();
        }

        public void ShowShopScreen()
        {
            ShowMainMenuScreen();
            HideMenuModalScreens();
            homeModalScreen?.ShowScreen();
        }

        public void ShowSettingsScreen()
        {
            ShowMainMenuScreen();
            HideMenuModalScreens();
            settingsModalScreen?.ShowScreen();
        }

        public void ShowLoginScreen()
        {
            ShowModalScreen(loginModalScreen, false);
        }

        public void ShowMainMenuScreen()
        {
            ShowModalScreen(mainMenuScreen, false);
        }

        public void ShowPlayerNamePopupScreen()
        {
            playerNamePopupScreen?.ShowScreen();
        }

        public void HidePlayerNamePopupScreen()
        {
            playerNamePopupScreen?.HideScreen();
        }

        public void ShowNetworkDisconnectedScreen()
        {
            networkDisconnectedScreen?.ShowScreen();
        }

        public void HideNetworkDisconnectedScreen()
        {
            networkDisconnectedScreen?.HideScreen();
        }

        public void ShowEULAScreen()
        {
            eulaModalScreen?.ShowScreen();
        }

        public void HideEULAScreen()
        {
            eulaModalScreen?.HideScreen();
        }

        public void ShowLoadingScreen()
        {
            ShowModalScreen(clientLoadingScreen, false);
        }

        public void HideLoadingScreen()
        {
            clientLoadingScreen?.HideScreen();
        }

        public void UpdateLoadingScreen()
        {
            clientLoadingScreen?.UpdateLoadingScreen();
        }

        public void ShowBufferScreen()
        {
            bufferScreen?.ShowScreen();
        }

        public void HideBufferScreen()
        {
            bufferScreen?.HideScreen();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                GameManager.Instance.SetCoins(GameManager.Instance.GameData.Coins + 100);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                GameManager.Instance.SetGems(GameManager.Instance.GameData.Gems + 10);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                GameManager.Instance.SetPlayerTrophies(GameManager.Instance.GameData.PlayerTrophies + 10);
            }

        }
    }
}