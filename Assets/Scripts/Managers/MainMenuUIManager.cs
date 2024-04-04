using System;
using System.Collections.Generic;
using KitchenKrapper;
using UI.Base;
using UI.Screen;
using UnityEngine;
using UnityEngine.UIElements;

namespace Managers
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuUIManager : MonoSingleton<MainMenuUIManager>
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
        public const string PopupPanelActiveClassName = "popup-panel";
        public const string PopupPanelInactiveClassName = "popup-panel--inactive";
        public const string ModalPanelActiveClassName = "modal-panel";
        public const string ModalPanelInactiveClassName = "modal-panel--inactive";

        // UI Elements and Stacks
        private UIDocument _mainMenuDocument;
        private readonly List<BaseScreen> _allModalScreens = new();
        private readonly Stack<BaseScreen> _screenStack = new();
        private readonly Stack<BaseScreen> _popupStack = new();
        private readonly Stack<BaseScreen> _overlayStack = new();

        protected override void Awake()
        {
            base.Awake();
            _mainMenuDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            SetupModalScreens();
        }

        private void SetupModalScreens()
        {
            // Populate the allModalScreens list
            _allModalScreens.Clear();
            _allModalScreens.AddRange(GetComponentsInChildren<BaseScreen>());

            // Show and hide all screens initially
            ShowAndHideAllScreens();
        }

        private void ShowAndHideAllScreens()
        {
            foreach (var screen in _allModalScreens)
            {
                screen.Show();
                screen.Hide();
            }
        }

        private static void HideAllScreens(Stack<BaseScreen> stack)
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
            // PrintScreenStack(stack);
        }

        private void HideScreen(Stack<BaseScreen> stack, BaseScreen screen)
        {
            if (screen == null || stack.Count == 0)
                return;

            HideParentScreen(screen, stack);

            if (stack.Count <= 0) return;
            stack.Pop().Hide();
            // PrintScreenStack(stack);
        }

        private void ShowScreen(BaseScreen screen)
        {
            switch (screen.GetScreenType())
            {
                case ScreenType.FullScreen:
                    ShowScreen(_screenStack, screen);
                    break;
                case ScreenType.Popup:
                    ShowScreen(_popupStack, screen);
                    break;
                case ScreenType.Overlay:
                    ShowScreen(_overlayStack, screen);
                    break;
                case ScreenType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HideScreen(BaseScreen screen)
        {
            switch (screen.GetScreenType())
            {
                case ScreenType.FullScreen:
                    HideScreen(_screenStack, screen);
                    break;
                case ScreenType.Popup:
                    HideScreen(_popupStack, screen);
                    break;
                case ScreenType.Overlay:
                    HideScreen(_overlayStack, screen);
                    break;
                case ScreenType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowParentScreen(BaseScreen screen, Stack<BaseScreen> stack)
        {
            if (screen == null)
                return;

            if (screen.GetParentScreen() == null) return;
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

        private void HideParentScreen(BaseScreen screen, Stack<BaseScreen> stack)
        {
            if (screen == null || stack.Count == 0)
                return;

            if (screen.GetParentScreen() == null) return;
            if (!stack.Contains(screen.GetParentScreen())) return;
            while (stack.Peek() != screen.GetParentScreen())
            {
                HideScreen(stack.Peek());
            }
        }

        private static void PrintScreenStack(Stack<BaseScreen> stack)
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
        public void ShowEulaScreen() => ShowScreen(eulaModalScreen);
        public void HideEulaScreen() => HideScreen(eulaModalScreen);
        public void ShowLoadingScreen() => ShowScreen(clientLoadingScreen);
        public void StopLoadingScreen() => clientLoadingScreen.StopLoadingScreen();
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
            return _mainMenuDocument;
        }
    }
}
