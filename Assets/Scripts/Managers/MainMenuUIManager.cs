using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuUIManager : Singleton<MainMenuUIManager>
    {
        // Serialized Fields
        [Header("Modal Menu Screens")]
        [SerializeField] private HomeScreen homeModalScreen;
        [SerializeField] private SettingsScreen settingsModalScreen;
        [SerializeField] private ShopScreen shopModalScreen;
        [SerializeField] private ClientLoadingScreen clientLoadingScreen;
        [SerializeField] private LoginScreen loginModalScreen;
        [SerializeField] private MainMenuScreen mainMenuScreen;

        [Header("Overlay Screens")]
        [SerializeField] private NetworkDisconnectedScreen networkDisconnectedScreen;
        [SerializeField] private BufferScreen bufferScreen;

        [Header("Pop-up Screens")]
        [SerializeField] private EULAScreen eulaModalScreen;
        [SerializeField] private PlayerNamePopupScreen playerNamePopupScreen;
        [SerializeField] private JoinPopupScreen joinPopupScreen;

        [Header("Toolbars")]
        [SerializeField] private OptionbarScreen optionbarScreen;

        // Constants
        public const string POPUP_PANEL_ACTIVE_CLASS_NAME = "popup-panel";
        public const string POPUP_PANEL_INACTIVE_CLASS_NAME = "popup-panel--inactive";
        public const string MODAL_PANEL_ACTIVE_CLASS_NAME = "modal-panel";
        public const string MODAL_PANEL_INACTIVE_CLASS_NAME = "modal-panel--inactive";

        // UI Elements and Stacks
        private UIDocument mainMenuDocument;
        private List<BaseScreen> allModalScreens = new List<BaseScreen>();
        private Stack<BaseScreen> screenStack = new Stack<BaseScreen>();
        private Stack<BaseScreen> popupStack = new Stack<BaseScreen>();
        private Stack<BaseScreen> overlayStack = new Stack<BaseScreen>();

        protected override void Awake()
        {
            base.Awake();
            mainMenuDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            SetupModalScreens();
        }

        private void SetupModalScreens()
        {
            // Populate the allModalScreens list
            allModalScreens.Clear();
            allModalScreens.AddRange(GetComponentsInChildren<BaseScreen>());

            // Show and hide all screens initially
            ShowAndHideAllScreens();
        }

        private void ShowAndHideAllScreens()
        {
            foreach (BaseScreen screen in allModalScreens)
            {
                screen.Show();
                screen.Hide();
            }
        }

        private void HideAllScreens(Stack<BaseScreen> stack)
        {
            if (stack.Count == 0)
                return;

            while (stack.Count > 0)
            {
                stack.Pop().Hide();
            }
        }

        private void ShowScreen(Stack<BaseScreen> stack, BaseScreen screen)
        {
            if (screen == null)
                return;

            HideAllScreens(stack);

            if (stack.Count > 0 && (stack.Peek() == screen || stack.Contains(screen)))
            {
                Debug.LogWarning("Screen already in stack: " + screen.name);
                return;
            }

            ShowParentScreen(screen, stack);
            stack.Push(screen);

            screen.Show();
            PrintScreenStack(stack);
        }

        private void HideScreen(Stack<BaseScreen> stack, BaseScreen screen)
        {
            if (screen == null || stack.Count == 0)
                return;

            HideParentScreen(screen, stack);

            if (stack.Count > 0)
            {
                stack.Pop().Hide();
                PrintScreenStack(stack);
            }
        }

        public void ShowScreen(BaseScreen screen)
        {
            switch (screen.GetScreenType())
            {
                case ScreenType.FullScreen:
                    ShowScreen(screenStack, screen);
                    break;
                case ScreenType.Popup:
                    ShowScreen(popupStack, screen);
                    break;
                case ScreenType.Overlay:
                    ShowScreen(overlayStack, screen);
                    break;
            }
        }

        public void HideScreen(BaseScreen screen)
        {
            switch (screen.GetScreenType())
            {
                case ScreenType.FullScreen:
                    HideScreen(screenStack, screen);
                    break;
                case ScreenType.Popup:
                    HideScreen(popupStack, screen);
                    break;
                case ScreenType.Overlay:
                    HideScreen(overlayStack, screen);
                    break;
            }
        }

        private void ShowParentScreen(BaseScreen screen, Stack<BaseScreen> stack)
        {
            if (screen == null)
                return;

            if (screen.GetParentScreen() != null)
            {
                if (stack.Contains(screen.GetParentScreen()))
                {
                    while (stack.Count > 0 && stack.Peek() != screen.GetParentScreen())
                    {
                        HideScreen(stack.Peek());
                    }
                }
                else
                {
                    ShowScreen(screen.GetParentScreen());
                }
            }
        }

        private void HideParentScreen(BaseScreen screen, Stack<BaseScreen> stack)
        {
            if (screen == null || stack.Count == 0)
                return;

            if (screen.GetParentScreen() != null)
            {
                if (stack.Contains(screen.GetParentScreen()))
                {
                    while (stack.Peek() != screen.GetParentScreen())
                    {
                        HideScreen(stack.Peek());
                    }
                }
            }
        }

        private void PrintScreenStack(Stack<BaseScreen> stack)
        {
            if (stack.Count == 0)
            {
                Debug.Log("Stack is empty");
                return;
            }

            Debug.Log("--------- SHOW " + stack.Peek() + " --------------");

            foreach (BaseScreen s in stack)
            {
                Debug.Log("Show: " + "[" + s.GetScreenType() + "]" + s.name);
            }

            Debug.Log("--------- END --------------");
        }

        public void ShowHomeScreen() => ShowScreen(homeModalScreen);
        public void ShowInventoryScreen() => ShowScreen(homeModalScreen);
        public void ShowShopScreen() => ShowScreen(shopModalScreen);
        public void ShowSettingsScreen() => ShowScreen(settingsModalScreen);
        public void ShowLoginScreen() => ShowScreen(loginModalScreen);
        public void ShowPlayerNamePopupScreen() => ShowScreen(playerNamePopupScreen);
        public void HidePlayerNamePopupScreen() => HideScreen(playerNamePopupScreen);
        public void ShowNetworkDisconnectedScreen() => ShowScreen(networkDisconnectedScreen);
        public void HideNetworkDisconnectedScreen() => HideScreen(networkDisconnectedScreen);
        public void ShowEULAScreen() => ShowScreen(eulaModalScreen);
        public void HideEULAScreen() => HideScreen(eulaModalScreen);
        public void ShowLoadingScreen() => ShowScreen(clientLoadingScreen);
        public void HideLoadingScreen() => HideScreen(clientLoadingScreen);
        public void UpdateLoadingScreen() => clientLoadingScreen.UpdateLoadingScreen();
        public void ShowBufferScreen() => ShowScreen(bufferScreen);
        public void HideBufferScreen() => HideScreen(bufferScreen);
        public void ShowJoinPopupScreen() => ShowScreen(joinPopupScreen);
        public void HideJoinPopupScreen() => HideScreen(joinPopupScreen);


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) GameManager.Instance.SetCoins(GameManager.Instance.PlayerData.Coins + 100);
            if (Input.GetKeyDown(KeyCode.G)) GameManager.Instance.SetGems(GameManager.Instance.PlayerData.Gems + 10);
            if (Input.GetKeyDown(KeyCode.P)) GameManager.Instance.SetPlayerTrophies(GameManager.Instance.PlayerData.PlayerTrophies + 10);
        }

        public UIDocument GetMainDocument()
        {
            return mainMenuDocument;
        }
    }
}
