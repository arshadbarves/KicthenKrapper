using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using BrunoMikoski.AnimationSequencer;
using System;

public class StartupUI : MonoBehaviour
{

    public static StartupUI Instance { get; private set; }

    [Header("Urls")]
    [SerializeField] private string m_termsOfUseUrl = "https://www.microsoft.com/en-us/servicesagreement/";
    [SerializeField] private string m_privacyPolicyUrl = "https://privacy.microsoft.com/en-us/privacystatement";

    [Header("Panels")]
    [SerializeField] private GameObject m_welcomePanel;
    [SerializeField] private GameObject m_networkErrorPanel;
    [SerializeField] private GameObject m_eulaPanel;

    // Getters and setters for the panels.
    public GameObject WelcomePanel { get { return m_welcomePanel; } }
    public GameObject NetworkErrorPanel { get { return m_networkErrorPanel; } }
    public GameObject EULAPanel { get { return m_eulaPanel; } }

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
        // Show the welcome panel.

        OnPanelShow(m_welcomePanel);
    }

    // Create a common method to handle the panel dismiss button.
    public void OnPanelDismiss(GameObject panel)
    {
        if (IsPanelActive(panel))
        {
            // Hide the panel.
            panel.SetActive(false);
        }
    }

    // Create a common method to handle the panel show button.
    public void OnPanelShow(GameObject panel)
    {
        if (!IsPanelActive(panel))
        {
            // Play the panel's animation.
            var animationSequencerController = panel.GetComponent<AnimationSequencerController>();

            if (animationSequencerController != null)
            {
                animationSequencerController.Play();
            }

            // Show the panel.
            panel.SetActive(true);
        }
    }

    // Create a common method to handle the panel toggle button.
    public bool IsPanelActive(GameObject panel)
    {
        return panel.activeSelf;
    }

    // Create a common method to handle the terms of use button that opens the terms of use URL.
    public void OnTermsOfUse()
    {
        Application.OpenURL(m_termsOfUseUrl);
    }

    // Create a common method to handle the privacy policy button that opens the privacy policy URL.
    public void OnPrivacyPolicy()
    {
        Application.OpenURL(m_privacyPolicyUrl);
    }

    // Create a common method to handle the network error panel retry button.
    public void OnNetworkErrorRetry()
    {
        // Hide the network error panel.
        OnPanelDismiss(m_networkErrorPanel);

        // Reload the scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowEULA()
    {
        // Show the EULA panel.
        OnPanelShow(m_eulaPanel);
    }

    // Create a common method to handle the EULA accept button.
    public void OnEULAAccept()
    {
        // Hide the EULA panel.
        OnPanelDismiss(m_eulaPanel);

        // Set the EULA accepted flag.
        GameDataSource.Instance.SetEULAAndPrivacyPolicyAccepted(true);

        ApplicationController.Instance.StartGame();
    }

    // Create a common method to handle the EULA decline button.
    public void OnEULADecline()
    {
        OnPanelDismiss(m_eulaPanel);
        // Quit the application.
        Application.Quit();
    }
}
