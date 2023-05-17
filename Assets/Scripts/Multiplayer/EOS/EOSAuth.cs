using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;
using static OpenIDManager;

public class EOSAuth : MonoBehaviour
{
    public static EOSAuth Instance { get; private set; }

    private string m_refreshToken;

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

    public void Login()
    {
        ShowLoader();
        string loginType = ClientPrefs.GetLoginType();
        if (loginType == ExternalCredentialType.OpenidAccessToken.ToString())
        {
            string refreshToken = ClientPrefs.GetOpenIDToken();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                print("Login with refresh token");
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
        UniversalUI.Instance.OnPanelShow(UniversalUI.Instance.WelcomePanel);
        UniversalUI.Instance.OnPanelHide(UniversalUI.Instance.TitlePanel);
    }

    private void ShowLoader()
    {
        UniversalUI.Instance.OnPanelShow(UniversalUI.Instance.LoadingPanel);
    }

    private void HideLoader()
    {
        UniversalUI.Instance.OnPanelHide(UniversalUI.Instance.LoadingPanel);
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
            print(EOSManager.Instance.GetProductUserId());
            EOSManager.Instance.ClearConnectId(EOSManager.Instance.GetProductUserId());
            ApplicationController.Instance.CleanupApplication();
            print("Logout Successful. [No User]");
            SceneLoaderWrapper.Instance.LoadScene(SceneType.Startup.ToString(), false);
            return;
        }

        EOSManager.Instance.StartLogout(EOSManager.Instance.GetLocalUserId(), (ref LogoutCallbackInfo data) =>
        {
            print("LogoutCallbackInfo");
            if (data.ResultCode == Result.Success)
            {
                print("Logout Successful. [" + data.ResultCode + "]");
                ApplicationController.Instance.CleanupApplication();
                SceneLoaderWrapper.Instance.LoadScene(SceneType.Startup.ToString(), false);
            }
            else
            {
                print("Logout Failed. [" + data.ResultCode + "]");
            }
        });

        print(EOSManager.Instance.GetLocalUserId());
    }

    // ------------------------------------------------------------------- //
    // --------------------- Login with OpenID Token --------------------- //
    // ------------------------------------------------------------------- //
    private void ConnectOpenID()
    {
        ShowLoader();
        OpenIDManager.Instance.RequestOAuth2Token(OnOpenIDTokenReceived);
    }

    private void OnOpenIDTokenReceived(TokenReader tokenContent)
    {
        if (tokenContent != null && tokenContent.access_token != null)
        {
            m_refreshToken = tokenContent.refresh_token;
            StartConnectLoginWithToken(ExternalCredentialType.OpenidAccessToken, tokenContent.access_token, null);
        }
        else
        {
            HideLoader();
            Debug.LogError("OpenID Login failed");
        }
    }
    // ---------------------------------------------------------------------- //
    // --------------------- Login with Device ID Token --------------------- //
    // ---------------------------------------------------------------------- //
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
            // TODO: Get the user's display name from the UI
            string displayName = "Guest" + Random.Range(0, 1000);
            StartConnectLoginWithToken(ExternalCredentialType.DeviceidAccessToken, null, displayName);
        }
        else
        {
            Debug.LogError("Connect Login failed: Failed to create Device Id");
        }
    }

    private void StartConnectLoginWithToken(ExternalCredentialType externalType, string token, string displayName = null)
    {
        EOSManager.Instance.StartConnectLoginWithOptions(externalType, token, displayName, (Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo) =>
        {
            if (connectLoginCallbackInfo.ResultCode == Result.Success)
            {
                print("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
                ClientPrefs.SetLoginType(externalType.ToString());
                ClientPrefs.SetOpenIDToken(m_refreshToken);
                HideLoader();
                ApplicationController.Instance.StartGame();
            }
            else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
            {
                // ask user if they want to connect; sample assumes they do
                EOSManager.Instance.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
                {
                    print("Creating new connect user");
                    if (createUserCallbackInfo.ResultCode == Result.Success)
                    {
                        print("New connect user created");
                        StartConnectLoginWithToken(externalType, token, displayName);
                    }
                    else
                    {
                        print("Failed to create new connect user");
                        HideLoader();
                    }
                });
            }
            else
            {
                print("Connect Login failed: " + connectLoginCallbackInfo.ResultCode);
                HideLoader();
            }
        });
    }
}
