using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class LoginScreen : Screen
    {
        private const string LoginButtonName = "login__login-button";
        private const string GuestButtonName = "login__guest-button";

        private Button loginButton;
        private Button guestButton;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            loginButton = root.Q<Button>(LoginButtonName);
            guestButton = root.Q<Button>(GuestButtonName);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();

            // add active style
            screen.AddToClassList(MainMenuUIManager.ModalPanelActiveClassName);
            screen.RemoveFromClassList(MainMenuUIManager.ModalPanelInactiveClassName);
        }

        public override void HideScreen()
        {
            base.HideScreen();

            // add inactive style
            screen.AddToClassList(MainMenuUIManager.ModalPanelInactiveClassName);
            screen.RemoveFromClassList(MainMenuUIManager.ModalPanelActiveClassName);
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            loginButton.clicked += OnLoginButtonClicked;
            guestButton.clicked += OnGuestButtonClicked;
        }

        protected override void UnregisterButtonCallbacks()
        {
            base.UnregisterButtonCallbacks();

            loginButton.clicked -= OnLoginButtonClicked;
            guestButton.clicked -= OnGuestButtonClicked;
        }

        private void OnLoginButtonClicked()
        {
            EOSAuth.Instance.LoginWithOpenID();
        }

        private void OnGuestButtonClicked()
        {
            EOSAuth.Instance.LoginWithDeviceId();
        }
    }
}