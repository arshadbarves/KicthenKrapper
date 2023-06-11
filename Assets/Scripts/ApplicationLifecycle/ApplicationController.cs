using System.Collections;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;

// This class is the entry point for the application. It is responsible for
// initializing the application and managing the application lifecycle.
// It is also responsible for managing the application's state.
// It is a singleton class.

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private UniversalUI m_universalUI;

    public UniversalUI UniversalUI { get { return m_universalUI; } }

    public static ApplicationController Instance { get; private set; }

    private GameStatus m_gameStatus = GameStatus.None;

    public GameStatus GameStatus
    {
        get { return m_gameStatus; }
        set { m_gameStatus = value; }
    }

    private void Awake()
    {
        // Check if there is already an instance of GameController.
        if (Instance == null)
        {
            // If not, set it to this.
            Instance = this;
        }
        // If instance already exists and it's not this:
        else if (Instance != this)
        {
            // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameController.
            Destroy(gameObject);
        }

        // Sets this to not be destroyed when reloading scene.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeApplication();
    }

    public void InitializeApplication()
    {
        UniversalUI.OnPanelShow(UniversalUI.TitlePanel);
        // Check if the user has accepted the EULA and privacy policy.
        if (!ClientPrefs.GetEULAAndPrivacyPolicyAccepted())
        {
            // Show the EULA and privacy policy.
            StartCoroutine(m_universalUI.ShowEULAAndPrivacyPolicy());
            return;
        }
        else
        {
            StartApp();
        }
    }

    public void StartApp()
    {
        // Set up and establish network connections to servers or other players.
        StartCoroutine(checkInternetConnection());

        EOSAuth.Instance.Login();

        ClientPrefs.Initialize();
    }

    public void StartGame()
    {
        // Start the game.
        m_gameStatus = GameStatus.GameStarted;
        m_universalUI.HideAllPanels();
        SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), false);
    }

    public void QuitGame()
    {
        // Quit the game.
        m_gameStatus = GameStatus.GameQuit;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator checkInternetConnection()
    {
        // Check if the device is connected to the internet.
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // Show the no internet connection panel.
            m_universalUI.OnPanelShow(m_universalUI.NetworkErrorPanel);
        }
        else
        {
            // Hide the no internet connection panel.
            m_universalUI.OnPanelHide(m_universalUI.NetworkErrorPanel);
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(checkInternetConnection());
    }

    private void ResetPlayerAuth()
    {
        var authInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
        var options = new Epic.OnlineServices.Auth.DeletePersistentAuthOptions();

        authInterface.DeletePersistentAuth(ref options, null, (ref DeletePersistentAuthCallbackInfo deletePersistentAuthCallbackInfo) =>
        {
            if (deletePersistentAuthCallbackInfo.ResultCode == Result.Success)
            {
                Debug.Log("Persistent auth deleted");
            }
            else
            {
                Debug.Log("Persistent auth not deleted");
            }
        });

        var connectInterface = EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
        var connectOptions = new Epic.OnlineServices.Connect.DeleteDeviceIdOptions();

        connectInterface.DeleteDeviceId(ref connectOptions, null, (ref DeleteDeviceIdCallbackInfo deleteDeviceIdCallbackInfo) =>
        {
            if (deleteDeviceIdCallbackInfo.ResultCode == Result.Success)
            {
                Debug.Log("Device ID deleted");
            }
            else
            {
                Debug.Log("Device ID not deleted");
            }
        });
    }

    public void CleanupApplication()
    {
        // Reset the Game Data
        GameDataSource.Instance.ResetGameData();

        // Reset the game status.
        m_gameStatus = GameStatus.None;

        // Reset the ClientPrefs.
        ClientPrefs.ResetClientPrefs();

        // Reset the Player Authenticaion.
        // ResetPlayerAuth();

        // Reset the application.
        InitializeApplication();

        // Destroy the Singleton instance.
        if (ApplicationManager.Instance != null)
        {
            Destroy(ApplicationManager.Instance.gameObject);
        }
    }
}
