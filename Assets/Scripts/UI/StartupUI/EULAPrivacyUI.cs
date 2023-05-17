using UnityEngine;
using UnityEngine.UI;

public class EULAPrivacyUI : MonoBehaviour
{
    [Header("Urls")]
    [SerializeField] private string m_termsOfUseUrl = "https://www.microsoft.com/en-us/servicesagreement/";
    [SerializeField] private string m_privacyPolicyUrl = "https://privacy.microsoft.com/en-us/privacystatement";

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button m_acceptEULAButton;
    [SerializeField] private Button m_declineEULAButton;
    [SerializeField] private Button m_termsOfUseButton;
    [SerializeField] private Button m_privacyPolicyButton;

    private void Awake()
    {
        // Set the EULA and privacy policy buttons.
        m_termsOfUseButton.onClick.AddListener(OnTermsOfUse);
        m_privacyPolicyButton.onClick.AddListener(OnPrivacyPolicy);

        // Set the EULA accept and decline buttons.
        m_acceptEULAButton.onClick.AddListener(OnEULAAccept);
        m_declineEULAButton.onClick.AddListener(OnEULADecline);

        closeButton.onClick.AddListener(OnEULADecline);
    }

    // A common method to handle the terms of use button that opens the terms of use URL.
    public void OnTermsOfUse()
    {
        Application.OpenURL(m_termsOfUseUrl);
    }

    // A common method to handle the privacy policy button that opens the privacy policy URL.
    public void OnPrivacyPolicy()
    {
        Application.OpenURL(m_privacyPolicyUrl);
    }

    // A common method to handle the EULA accept button.
    public void OnEULAAccept()
    {
        Hide();
        // Set the EULA accepted flag.
        ClientPrefs.SetEULAAndPrivacyPolicyAccepted(true);
        ApplicationController.Instance.StartApp();
    }

    // A common method to handle the EULA decline button.
    public void OnEULADecline()
    {
        Hide();
        // Quit the application.
        ApplicationController.Instance.QuitGame();
    }
    public void OnLogoutButtonClicked()
    {
        EOSAuth.Instance.Logout();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
