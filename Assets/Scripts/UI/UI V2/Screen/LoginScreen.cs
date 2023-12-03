using Managers;
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

            loginButton = root.Q<Button>(LOGIN_BUTTON_NAME);
            guestButton = root.Q<Button>(GUEST_BUTTON_NAME);
        }

        public override void Show()
        {
            base.Show();

            // add active style
            screen.AddToClassList(MainMenuUIManager.MODAL_PANEL_ACTIVE_CLASS_NAME);
            screen.RemoveFromClassList(MainMenuUIManager.MODAL_PANEL_INACTIVE_CLASS_NAME);
        }

        public override void Hide()
        {
            base.Hide();

            // add inactive style
            screen.AddToClassList(MainMenuUIManager.MODAL_PANEL_INACTIVE_CLASS_NAME);
            screen.RemoveFromClassList(MainMenuUIManager.MODAL_PANEL_ACTIVE_CLASS_NAME);
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