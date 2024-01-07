using Managers;
using Multiplayer.EOS;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button closeButton;

        // Reference to UI elements
        public Toggle musicToggle;
        public Toggle soundEffectsToggle;
        public Image musicIcon;
        public Image soundEffectsIcon;

        public Sprite musicOnIcon;
        public Sprite musicOffIcon;
        public Sprite soundOnIcon;
        public Sprite soundOffIcon;

        private void Start()
        {
            // Initialize UI elements based on audio manager settings
            musicToggle.isOn = !AudioManager.Instance.IsMusicMuted();
            soundEffectsToggle.isOn = !AudioManager.Instance.AreSoundEffectsMuted();

            UpdateMusicIcon(!AudioManager.Instance.IsMusicMuted());
            UpdateSoundEffectsIcon(!AudioManager.Instance.AreSoundEffectsMuted());
        }

        private void Awake()
        {
            logoutButton.onClick.AddListener(OnLogoutButtonClicked);
            closeButton.onClick.AddListener(Hide);
        }

        // Method to handle music toggle
        public void OnMusicToggleChanged(bool isOn)
        {
            AudioManager.Instance.ToggleMusicMute(!isOn);
            UpdateMusicIcon(isOn);
        }

        // Method to handle sound effects toggle
        public void OnSoundEffectsToggleChanged(bool isOn)
        {
            AudioManager.Instance.ToggleSoundEffectsMute(!isOn);
            UpdateSoundEffectsIcon(isOn);
        }

        // Method to update music icon based on mute status
        private void UpdateMusicIcon(bool isOn)
        {
            musicIcon.sprite = isOn ? musicOnIcon : musicOffIcon;
        }

        // Method to update sound effects icon based on mute status
        private void UpdateSoundEffectsIcon(bool isOn)
        {
            soundEffectsIcon.sprite = isOn ? soundOnIcon : soundOffIcon;
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
}