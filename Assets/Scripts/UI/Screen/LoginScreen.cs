using Managers;
using Multiplayer.EOS;
using UI.Base;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class LoginScreen : BaseScreen
    {
        private const string LOGIN_BUTTON_NAME = "login__login-button";
        private const string GUEST_BUTTON_NAME = "login__guest-button";

        private Button loginButton;
        private Button guestButton;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            loginButton = Root.Q<Button>(LOGIN_BUTTON_NAME);
            guestButton = Root.Q<Button>(GUEST_BUTTON_NAME);
        }

        public override void Show()
        {
            base.Show();

            // add active style
            Screen.AddToClassList(MainMenuUIManager.ModalPanelActiveClassName);
            Screen.RemoveFromClassList(MainMenuUIManager.ModalPanelInactiveClassName);
        }

        public override void Hide()
        {
            base.Hide();

            // add inactive style
            Screen.AddToClassList(MainMenuUIManager.ModalPanelInactiveClassName);
            Screen.RemoveFromClassList(MainMenuUIManager.ModalPanelActiveClassName);
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