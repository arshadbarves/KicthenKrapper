using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject popupNamePanel;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;

    [Header("Multiplayer Toggle")]
    [SerializeField] private Toggle multiplayerToggle;
    [SerializeField] private Image multiplayerToggleBackground;
    [SerializeField] private TextMeshProUGUI multiplayerText;
    // [SerializeField] private Button MapButton;
    // [SerializeField] private Button ShopButton;
    // [SerializeField] private GameObject MapPanel;
    // [SerializeField] private GameObject ShopPanel;

    private void Awake()
    {
        // playButton.onClick.AddListener(OnPlayButtonClicked);
        multiplayerToggle.onValueChanged.AddListener(OnMultiplayerToggleValueChanged);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        // MapButton.onClick.AddListener(OnNavigationButtonClicked(MapPanel));
        // ShopButton.onClick.AddListener(OnNavigationButtonClicked(ShopPanel));
    }

    private void Start()
    {
        OnMultiplayerToggleValueChanged(GameDataSource.playMultiplayer);
    }

    private void OnMultiplayerToggleValueChanged(bool isMultiplayer)
    {
        GameDataSource.playMultiplayer = isMultiplayer;

        if (GameDataSource.playMultiplayer)
        {
            multiplayerText.text = "Multiplayer";
            // Set Toggle Background Color to #FE89FE
            multiplayerToggleBackground.color = new Color(254f / 255f, 137f / 255f, 254f / 255f);
        }
        else
        {
            multiplayerText.text = "Singleplayer";
            // Set Toggle Background Color to #FFC0CB
            multiplayerToggleBackground.color = new Color(255f / 255f, 192f / 255f, 203f / 255f);
        }
    }

    private UnityAction OnNavigationButtonClicked(GameObject mapPanel)
    {
        return () => ShowPanel(mapPanel);
    }

    private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        SceneLoaderWrapper.Instance.LoadScene(SceneType.MultiplayerMenu.ToString(), false);
    }

    public void OnSettingsButtonClicked()
    {
        ShowPanel(settingsPanel);
    }

    public void ShowPopupNamePanel()
    {
        ShowPanel(popupNamePanel);
    }
}
