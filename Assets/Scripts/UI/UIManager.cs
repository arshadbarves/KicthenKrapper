using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;
using Managers;

namespace KitchenKrapper
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Others")]
        [SerializeField] private float welcomePanelDisplayTime = 5.0f;

        [Header("Buttons")]
        [SerializeField] private Button loginButton;
        [SerializeField] private Button guestLoginButton;
        [SerializeField] private Button networkErrorRetryButton;

        [Header("Panels")]
        [SerializeField] private GameObject welcomePanel;
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject eulaPanel;
        [SerializeField] private GameObject networkErrorPanel;
        [SerializeField] private GameObject loadingPanel;

        // Getters for the panels.
        public GameObject EULAPanel { get { return eulaPanel; } }
        public GameObject NetworkErrorPanel { get { return networkErrorPanel; } }
        public GameObject WelcomePanel { get { return welcomePanel; } }
        public GameObject TitlePanel { get { return titlePanel; } }
        public GameObject LoadingPanel { get { return loadingPanel; } }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            loginButton.onClick.AddListener(OnLogin);
            guestLoginButton.onClick.AddListener(OnGuestLogin);
            networkErrorRetryButton.onClick.AddListener(OnNetworkErrorRetry);
        }

        public IEnumerator TitlePanelDisplayTime()
        {
            yield return new WaitForSeconds(welcomePanelDisplayTime);
            OnPanelHide(welcomePanel);
            GameManager.Instance.StartGame();
        }

        public void OnLogin()
        {
            EOSAuth.Instance.LoginWithOpenID();
        }

        public void OnGuestLogin()
        {
            EOSAuth.Instance.LoginWithDeviceId();
        }

        public bool IsPanelActive(GameObject panel)
        {
            return panel.activeSelf;
        }

        public void OnPanelHide(GameObject panel)
        {
            if (IsPanelActive(panel))
            {
                panel.SetActive(false);
            }
        }

        public void OnPanelShow(GameObject panel)
        {
            if (!IsPanelActive(panel))
            {
                panel.SetActive(true);
            }
        }

        public void OnPanelShowOnDelay(GameObject panel, float delay)
        {
            StartCoroutine(OnPanelShowOnDelayCoroutine(panel, delay));
        }

        private IEnumerator OnPanelShowOnDelayCoroutine(GameObject panel, float delay)
        {
            yield return new WaitForSeconds(delay);
            OnPanelShow(panel);
        }

        public void OnNetworkErrorRetry()
        {
            OnPanelHide(networkErrorPanel);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void HideAllPanels()
        {
            OnPanelHide(WelcomePanel);
            OnPanelHide(TitlePanel);
        }
    }
}