using System;
using SceneManagement;
using UI.Base;
using UnityEngine.UIElements;
using UnityEngine;

namespace KitchenKrapper
{
    public class NetworkDisconnectedScreen : BaseScreen
    {

        public static event Action NetworkDisconnectedScreenShown;
        public static event Action NetworkDisconnectedScreenHidden;

        private const string RECONNECT_BUTTON_NAME = "network-disconnected__reconnect-button";
        private const string NETWORK_DISCONNECTED_ICON_NAME = "network-disconnected__icon";

        private Button reconnectButton;
        private VisualElement networkDisconnectedIcon;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            reconnectButton = Root.Q<Button>(RECONNECT_BUTTON_NAME);
            networkDisconnectedIcon = Root.Q<VisualElement>(NETWORK_DISCONNECTED_ICON_NAME);
        }

        public void EnablePickable()
        {
            // screen.pickingMode = PickingMode.Position;
        }

        public void DisablePickable()
        {
            // screen.pickingMode = PickingMode.Ignore;
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            reconnectButton.clicked += OnReconnectButtonClicked;
        }

        protected override void UnregisterButtonCallbacks()
        {
            base.UnregisterButtonCallbacks();

            reconnectButton.clicked -= OnReconnectButtonClicked;
        }

        private void OnReconnectButtonClicked()
        {
            SceneLoaderWrapper.Instance.ReloadScene();
        }

        public override void Show()
        {
            base.Show();
            NetworkDisconnectedScreenShown?.Invoke();
        }

        public override void Hide()
        {
            base.Hide();
            NetworkDisconnectedScreenHidden?.Invoke();
        }

        public void ShowReconnectButton()
        {
            reconnectButton.style.visibility = Visibility.Visible;
        }

        public void HideReconnectButton()
        {
            reconnectButton.style.visibility = Visibility.Hidden;
        }

        private void Update()
        {
            if (IsVisible())
            {
                networkDisconnectedIcon.style.opacity = Mathf.PingPong(Time.time, 0.5f);
            }
        }
    }
}
