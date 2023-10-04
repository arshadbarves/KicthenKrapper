using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class EULAPrivacyUI : MonoBehaviour
    {
        [Header("Urls")]
        [SerializeField] private string termsOfUseUrl = "https://www.microsoft.com/en-us/servicesagreement/";
        [SerializeField] private string privacyPolicyUrl = "https://privacy.microsoft.com/en-us/privacystatement";

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button acceptEULAButton;
        [SerializeField] private Button declineEULAButton;
        [SerializeField] private Button termsOfUseButton;
        [SerializeField] private Button privacyPolicyButton;

        private void Awake()
        {
            InitializeButtonListeners();
        }

        private void InitializeButtonListeners()
        {
            termsOfUseButton.onClick.AddListener(OpenTermsOfUse);
            privacyPolicyButton.onClick.AddListener(OpenPrivacyPolicy);
            acceptEULAButton.onClick.AddListener(AcceptEULA);
            declineEULAButton.onClick.AddListener(DeclineEULA);
            closeButton.onClick.AddListener(DeclineEULA);
        }

        public void OpenTermsOfUse()
        {
            Application.OpenURL(termsOfUseUrl);
        }

        public void OpenPrivacyPolicy()
        {
            Application.OpenURL(privacyPolicyUrl);
        }

        public void AcceptEULA()
        {
            Hide();
            ClientPrefs.SetEULAAndPrivacyPolicyAccepted(true);
            GameManager.Instance.StartApp();
        }

        public void DeclineEULA()
        {
            Hide();
            GameManager.Instance.QuitGame();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}