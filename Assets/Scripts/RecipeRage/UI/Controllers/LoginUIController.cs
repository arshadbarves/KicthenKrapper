using RecipeRage.Managers;
using RecipeRage.UI.Base;
using RecipeRage.Utilities;
using UnityEngine.UIElements;

namespace RecipeRage.UI.Controllers
{
    public class LoginUIController : UIScreen
    {
        private const string DeviceIdWarningText = "Logging in with a device ID may lead to data loss if the app is uninstalled or its data cleared. We advise using Facebook login to save your progress. Do you want to continue?";

        private VisualElement _leftPanel;
        private VisualElement _rightPanel;
        private Button _loginWithDeviceIdButton;
        private Button _loginWithFacebookButton;
        private Button _closeButton;

        // Classes
        private const string LeftPanelActiveClass = "leftPanel--active";
        private const string RightPanelActiveClass = "rightPanel--active";

        protected override void Awake()
        {
            base.Awake();
            
            _leftPanel = Root.Q<VisualElement>("leftPanel");
            _rightPanel = Root.Q<VisualElement>("rightPanel");
            _loginWithDeviceIdButton = Root.Q<Button>("loginWithDeviceIdButton");
            _loginWithFacebookButton = Root.Q<Button>("loginWithFacebookButton");
            _closeButton = Root.Q<Button>("closeButton");

            _loginWithDeviceIdButton.clicked += OnLoginWithDeviceId;
            _loginWithFacebookButton.clicked += OnLoginWithFacebook;
            _closeButton.clicked += Hide;
        }

        private void OnDestroy()
        {
            _loginWithDeviceIdButton.clicked -= OnLoginWithDeviceId;
            _loginWithFacebookButton.clicked -= OnLoginWithFacebook;
            _closeButton.clicked -= OnClose;
        }

        private static void OnLoginWithDeviceId()
        {
            UIManager.Instance.ShowWarningPopup(DeviceIdWarningText, OnContinue);
        }

        private static async void OnContinue(bool continueWithDeviceId)
        {
            if (continueWithDeviceId)
            {
                UIManager.Instance.HideWarningPopup(OnContinue);
                await Auth.StartLogin(AuthType.DeviceId);
            }
            else
            {
                UIManager.Instance.HideWarningPopup(OnContinue);
            }
        }

        private static async void OnLoginWithFacebook()
        {
            await Auth.StartLogin(AuthType.Facebook);
        }

        public override void Show()
        {
            base.Show();
            AnimateToggleClass(_leftPanel, LeftPanelActiveClass);
            AnimateToggleClass(_rightPanel, RightPanelActiveClass);
        }

        public override void Hide()
        {
            AnimateToggleClass(_leftPanel, LeftPanelActiveClass, CheckAndHide);
            AnimateToggleClass(_rightPanel, RightPanelActiveClass, CheckAndHide);
        }

        private void CheckAndHide()
        {
            if (_leftPanel.ClassListContains(LeftPanelActiveClass) &&
                _rightPanel.ClassListContains(RightPanelActiveClass)) return;
            base.Hide();
        }
        
        private void OnClose()
        {
            UIManager.Instance.HideLoginScreen();
        }
    }
}