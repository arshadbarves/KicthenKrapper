using System.Collections;
using BrunoMikoski.AnimationSequencer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UniversalUI : MonoBehaviour
{
    public static UniversalUI Instance { get; private set; }


    [Header("Others")]
    [SerializeField] private float m_welcomePanelDisplayTime = 5.0f;

    [Header("Buttons")]

    [SerializeField] private Button m_loginButton;
    [SerializeField] private Button m_guestLoginButton;
    [SerializeField] private Button m_networkErrorRetryButton;

    [Header("Panels")]

    [SerializeField] private GameObject m_welcomePanel;
    [SerializeField] private GameObject m_titlePanel;
    [SerializeField] private GameObject m_eulaPanel;
    [SerializeField] private GameObject m_networkErrorPanel;
    [SerializeField] private GameObject m_loadingPanel;

    // Getters and setters for the panels.

    public GameObject EULAPanel { get { return m_eulaPanel; } }
    public GameObject NetworkErrorPanel { get { return m_networkErrorPanel; } }
    public GameObject WelcomePanel { get { return m_welcomePanel; } }
    public GameObject TitlePanel { get { return m_titlePanel; } }
    public GameObject LoadingPanel { get { return m_loadingPanel; } }

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
        m_loginButton.onClick.AddListener(OnLogin);
        m_guestLoginButton.onClick.AddListener(OnGuestLogin);
        m_networkErrorRetryButton.onClick.AddListener(OnNetworkErrorRetry);
    }

    public IEnumerator TitlePanelDisplayTime()
    {
        yield return new WaitForSeconds(m_welcomePanelDisplayTime);
        // Hide the welcome panel.
        OnPanelHide(WelcomePanel);
        ApplicationController.Instance.StartGame();
    }
    public void OnLogin()
    {
        EOSAuth.Instance.LoginWithOpenID();
    }

    public void OnGuestLogin()
    {
        EOSAuth.Instance.LoginWithDeviceId();
    }

    // A common method to handle the panel toggle button.
    public bool IsPanelActive(GameObject panel)
    {
        return panel.activeSelf;
    }

    public void OnPanelHide(GameObject panel)
    {
        if (IsPanelActive(panel))
        {
            // Hide the panel.
            panel.SetActive(false);
        }
    }

    // A common method to handle the panel show button.
    public void OnPanelShow(GameObject panel)
    {
        if (!IsPanelActive(panel))
        {
            // Play the panel's animation.
            var animationSequencerController = panel.GetComponent<AnimationSequencerController>();

            if (animationSequencerController != null)
            {
                animationSequencerController.Kill();
                animationSequencerController.Play();
            }

            // Show the panel.
            panel.SetActive(true);
        }
    }

    // A common method to handle the network error panel retry button.
    public void OnNetworkErrorRetry()
    {
        // Hide the network error panel.
        OnPanelHide(m_networkErrorPanel);

        // Reload the scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator ShowEULAAndPrivacyPolicy()
    {
        yield return new WaitForSeconds(1.0f);
        // Show the EULA and privacy policy.
        OnPanelShow(EULAPanel);
    }

    public void HideAllPanels()
    {
        OnPanelHide(WelcomePanel);
        OnPanelHide(TitlePanel);
    }
}
