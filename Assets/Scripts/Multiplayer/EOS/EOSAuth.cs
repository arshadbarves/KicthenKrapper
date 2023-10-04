using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;

namespace KitchenKrapper
{
    public class EOSAuth : MonoBehaviour
    {
        public static EOSAuth Instance { get; private set; }

        private string refreshToken;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);
        }

        public bool IsLoggedIn()
        {
            return EOSManager.Instance.GetLocalUserId() != null;
        }

        public ProductUserId GetProductUserId()
        {
            return EOSManager.Instance.GetProductUserId();
        }

        public void Login()
        {
            ShowLoader();
            string loginType = ClientPrefs.GetLoginType();
            if (loginType == ExternalCredentialType.OpenidAccessToken.ToString())
            {
                string refreshToken = ClientPrefs.GetOpenIDToken();
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    Debug.Log("Login with refresh token");
                    OpenIDManager.Instance.GetAccessToken(refreshToken, OnOpenIDTokenReceived);
                }
                else
                {
                    Debug.LogError("OpenID Login failed - no refresh token");
                    ShowLoginPage();
                }
            }
            else if (loginType == ExternalCredentialType.DeviceidAccessToken.ToString())
            {
                LoginWithDeviceId();
            }
            else
            {
                Debug.LogError("Login failed - no login type");
                ShowLoginPage();
            }
        }

        private void ShowLoginPage()
        {
            HideLoader();
            MainMenuUIManager.Instance.ShowLoginScreen();
        }

        private void ShowLoader()
        {
            MainMenuUIManager.Instance.ShowBufferScreen();
        }

        private void HideLoader()
        {
            MainMenuUIManager.Instance.HideBufferScreen();
        }

        public void LoginWithOpenID()
        {
            ConnectOpenID();
        }

        public void LoginWithDeviceId()
        {
            ConnectDeviceId();
        }

        public void Logout()
        {
            if (EOSManager.Instance.GetLocalUserId() == null)
            {
                Debug.Log(EOSManager.Instance.GetProductUserId());
                EOSManager.Instance.ClearConnectId(EOSManager.Instance.GetProductUserId());
                GameManager.Instance.CleanupApplication();
                Debug.Log("Logout Successful. [No User]");
                SceneLoaderWrapper.Instance.LoadScene(SceneType.Startup.ToString(), false);
                return;
            }

            EOSManager.Instance.StartLogout(EOSManager.Instance.GetLocalUserId(), (ref LogoutCallbackInfo data) =>
            {
                Debug.Log("LogoutCallbackInfo");
                if (data.ResultCode == Result.Success)
                {
                    Debug.Log("Logout Successful. [" + data.ResultCode + "]");
                    GameManager.Instance.CleanupApplication();
                    SceneLoaderWrapper.Instance.LoadScene(SceneType.Startup.ToString(), false);
                }
                else
                {
                    Debug.Log("Logout Failed. [" + data.ResultCode + "]");
                }
            });

            Debug.Log(EOSManager.Instance.GetLocalUserId());
        }

        private void ConnectOpenID()
        {
            ShowLoader();
            OpenIDManager.Instance.RequestOAuth2Token(OnOpenIDTokenReceived);
        }

        private void OnOpenIDTokenReceived(TokenReader tokenContent)
        {
            if (tokenContent != null && tokenContent.access_token != null)
            {
                refreshToken = tokenContent.refresh_token;
                StartConnectLoginWithToken(ExternalCredentialType.OpenidAccessToken, tokenContent.access_token, null);
            }
            else
            {
                HideLoader();
                Debug.LogError("OpenID Login failed");
            }
        }

        private void ConnectDeviceId()
        {
            var connectInterface = EOSManager.Instance.GetEOSConnectInterface();
            var options = new Epic.OnlineServices.Connect.CreateDeviceIdOptions()
            {
                DeviceModel = SystemInfo.deviceModel
            };

            connectInterface.CreateDeviceId(ref options, null, CreateDeviceCallback);
        }

        private void CreateDeviceCallback(ref Epic.OnlineServices.Connect.CreateDeviceIdCallbackInfo callbackInfo)
        {
            if (callbackInfo.ResultCode == Result.Success || callbackInfo.ResultCode == Result.DuplicateNotAllowed)
            {
                string displayName = "Player" + Random.Range(0, 1000);
                // EOSManager.Instance.StartConnectLoginWithOptions(ExternalCredentialType.DeviceidAccessToken, null, displayName, ConnectLoginTokenCallback);
                StartConnectLoginWithToken(ExternalCredentialType.DeviceidAccessToken, null, displayName);
            }
            else
            {
                Debug.LogError("Connect Login failed: Failed to create Device Id");
            }
        }

        private void ConnectLoginTokenCallback(Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo)
        {
            if (connectLoginCallbackInfo.ResultCode == Result.Success)
            {
                Debug.Log("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
                StartGame();
            }
            else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
            {
                // ask user if they want to connect; sample assumes they do
                EOSManager.Instance.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
                {
                    Debug.Log("Creating new connect user");
                    if (createUserCallbackInfo.ResultCode == Result.Success)
                    {
                        Debug.Log("New connect user created");
                        StartConnectLoginWithToken(ExternalCredentialType.DeviceidAccessToken, null);
                        // StartGame();
                    }
                    else
                    {
                        Debug.Log("Failed to create new connect user");
                        HideLoader();
                    }
                });
            }
            else
            {
                Debug.Log("Connect Login failed: " + connectLoginCallbackInfo.ResultCode);
                HideLoader();
            }
        }

        private void StartGame()
        {
            ClientPrefs.SetLoginType(ExternalCredentialType.DeviceidAccessToken.ToString());
            ClientPrefs.SetOpenIDToken(refreshToken);
            HideLoader();
            GameManager.Instance.StartGame();
        }

        private void StartConnectLoginWithToken(ExternalCredentialType externalType, string token, string displayName = null)
        {
            EOSManager.Instance.StartConnectLoginWithOptions(externalType, token, displayName, (Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo) =>
            {
                if (connectLoginCallbackInfo.ResultCode == Result.Success)
                {
                    Debug.Log("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
                    ClientPrefs.SetLoginType(externalType.ToString());
                    ClientPrefs.SetOpenIDToken(refreshToken);
                    HideLoader();
                    GameManager.Instance.StartGame();
                }
                else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
                {
                    EOSManager.Instance.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
                    {
                        Debug.Log("Creating new connect user");
                        if (createUserCallbackInfo.ResultCode == Result.Success)
                        {
                            Debug.Log("New connect user created");
                            StartConnectLoginWithToken(externalType, token, displayName);
                        }
                        else
                        {
                            Debug.Log("Failed to create new connect user");
                            HideLoader();
                        }
                    });
                }
                else
                {
                    Debug.Log("Connect Login failed: " + connectLoginCallbackInfo.ResultCode);
                    HideLoader();
                }
            });
        }
    }
}
