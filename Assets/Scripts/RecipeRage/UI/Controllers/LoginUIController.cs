using RecipeRage.Multiplayer.NakamaServer;
using RecipeRage.UI.Base;
using UnityEngine.UIElements;

namespace RecipeRage.UI.Controllers
{
    public class LoginUIController : UIScreen
    {
        private Button _loginWithDeviceIdButton;
        private Button _loginWithFacebookButton;
        
        protected override void Awake()
        {
            base.Awake();
            
            _loginWithDeviceIdButton = Root.Q<Button>("loginWithDeviceIdButton");
            _loginWithFacebookButton = Root.Q<Button>("loginWithFacebookButton");
            
            _loginWithDeviceIdButton.clicked += OnLoginWithDeviceId;
            _loginWithFacebookButton.clicked += OnLoginWithFacebook;
        }

        private void OnLoginWithDeviceId()
        {
#if NAKAMA_MULTIPLAYER
            NakamaManager.LoginWithDeviceId();
#endif
        }

        private void OnLoginWithFacebook()
        {
            // NakamaAuth.LoginWithFacebook();
        }
    }
}