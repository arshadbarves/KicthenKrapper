using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject popupSettingsPanel;
        [SerializeField] private GameObject popupNamePanel;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;

        [Header("Multiplayer Toggle")]
        [SerializeField] private Toggle multiplayerToggle;
        [SerializeField] private Image multiplayerToggleBackground;
        [SerializeField] private TextMeshProUGUI multiplayerText;

        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            multiplayerToggle.onValueChanged.AddListener(OnMultiplayerToggleValueChanged);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }

        private void Start()
        {
            OnMultiplayerToggleValueChanged(GameDataSource.PlayMultiplayer);
        }

        private void OnMultiplayerToggleValueChanged(bool isMultiplayer)
        {
            GameDataSource.PlayMultiplayer = isMultiplayer;

            if (GameDataSource.PlayMultiplayer)
            {
                multiplayerText.text = "Multiplayer";
                multiplayerToggleBackground.color = new Color(254f / 255f, 137f / 255f, 254f / 255f);
            }
            else
            {
                multiplayerText.text = "Singleplayer";
                multiplayerToggleBackground.color = new Color(255f / 255f, 192f / 255f, 203f / 255f);
            }
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
            ShowPanel(popupSettingsPanel);
        }

        public void ShowPopupNamePanel()
        {
            ShowPanel(popupNamePanel);
        }
    }
}