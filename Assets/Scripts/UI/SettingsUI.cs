using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        logoutButton.onClick.AddListener(OnLogoutButtonClicked);
        closeButton.onClick.AddListener(Hide);
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
