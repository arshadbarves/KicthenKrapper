using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Networking;

// This class is the entry point for the application. It is responsible for
// initializing the application and managing the application lifecycle.
// It is also responsible for managing the application's state.
// It is a singleton class.

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private StartupUI m_startupUI;
    [SerializeField] private float m_welcomePanelDisplayTime = 5.0f;

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
        // Initialize the application.
        InitializeApplication();
    }

    private void InitializeApplication()
    {
        // Start couroutines for welcome panel display time.
        // Load the game's configuration file and set up default settings.
        // LoadConfigurationFile();

        // Initialize the game's UI, including the main menu and HUD.
        // InitializeUI();

        // Set up and establish network connections to servers or other players.
        InitializeNetworkConnection();

        // Set up audio listeners and load audio assets.
        // InitializeAudio();

        // Load data files and initialize game variables and data structures.
        // InitializeGameData();

        // Initialize analytics and event tracking.
        // InitializeAnalytics();

        // Set up localization and translation systems.
        // InitializeLocalization();

        // Initialize logging and debugging tools.
        // InitializeLogging();

        // Set up crash and error reporting systems.
        // InitializeCrashReporting();

        // Initialize performance monitoring tools and set up metrics collection.
        // InitializePerformanceMonitoring();

        // Check if the user has accepted the EULA and privacy policy.
        if (!GameDataSource.Instance.GetEULAAndPrivacyPolicyAccepted())
        {
            // Show the EULA and privacy policy.
            StartCoroutine(ShowEULAAndPrivacyPolicy());
        }
        else
        {
            // Start the game.
            StartGame();
        }
    }

    private void InitializeNetworkConnection()
    {
        StartCoroutine(checkInternetConnection());
    }

    private IEnumerator ShowEULAAndPrivacyPolicy()
    {
        yield return new WaitForSeconds(1.0f);
        // Show the EULA and privacy policy.
        m_startupUI.OnPanelShow(m_startupUI.EULAPanel);
    }

    public void StartGame()
    {
        // Start the game.
        m_gameStatus = GameStatus.GameStarted;
        StartCoroutine(WelcomePanelDisplayTime());
    }

    private IEnumerator WelcomePanelDisplayTime()
    {
        yield return new WaitForSeconds(m_welcomePanelDisplayTime);
        // Hide the welcome panel.
        m_startupUI.OnPanelDismiss(m_startupUI.WelcomePanel);

        // Load the main menu scene if the high score is greater than 0 else load the tutorial scene.
        // if (GameDataSource.Instance.GetHighestTrophyCount() > 0)
        // {
        //     SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), GameDataSource.Instance.UseNetworkSceneManager());
        // }
        // else
        // {
        //     SceneLoaderWrapper.Instance.LoadScene(SceneType.Tutorial.ToString(), GameDataSource.Instance.UseNetworkSceneManager());
        // }

        // Load the main menu scene.
        SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), GameDataSource.Instance.UseNetworkSceneManager());
    }

    private IEnumerator checkInternetConnection()
    {
        // Check if the device is connected to the internet.
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // Show the no internet connection panel.
            m_startupUI.OnPanelShow(m_startupUI.NetworkErrorPanel);
        }
        else
        {
            // Hide the no internet connection panel.
            m_startupUI.OnPanelDismiss(m_startupUI.NetworkErrorPanel);
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(checkInternetConnection());
    }
}
