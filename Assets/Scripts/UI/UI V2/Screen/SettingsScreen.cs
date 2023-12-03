using System;
using Managers;
using MyUILibrary;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class SettingsScreen : BaseScreen
    {
        private const string SETTINGS_PANEL_NAME = "settings__panel";
        private const string SOUND_EFFECTS_SLIDER_NAME = "sound-effect__slider";
        private const string MUSIC_SLIDER_NAME = "music__slider";
        private const string RATE_US_BUTTON_NAME = "rate-us__button";
        private const string SUPPORT_BUTTON_NAME = "support__button";
        private const string LOGOUT_BUTTON_NAME = "logout__button";

        private VisualElement settingPanel;
        private SlideToggle soundEffectSlider;
        private SlideToggle musicSlider;
        private Button rateUsButton;
        private Button supportButton;
        private Button logoutButton;

        private bool isSettingPanelActive = false;

        private void Start()
        {
            GameManager.GameDataUpdated += OnGameDataUpdated;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.GameDataUpdated -= OnGameDataUpdated;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            settingPanel = root.Q<VisualElement>(SETTINGS_PANEL_NAME);
            soundEffectSlider = root.Q<SlideToggle>(SOUND_EFFECTS_SLIDER_NAME);
            musicSlider = root.Q<SlideToggle>(MUSIC_SLIDER_NAME);
            rateUsButton = root.Q<Button>(RATE_US_BUTTON_NAME);
            supportButton = root.Q<Button>(SUPPORT_BUTTON_NAME);
            logoutButton = root.Q<Button>(LOGOUT_BUTTON_NAME);
        }

        public override void Show()
        {
            base.Show();

            OnGameDataUpdated();
            isSettingPanelActive = true;
            // add active style
            settingPanel.AddToClassList(MainMenuUIManager.MODAL_PANEL_ACTIVE_CLASS_NAME);
            settingPanel.RemoveFromClassList(MainMenuUIManager.MODAL_PANEL_INACTIVE_CLASS_NAME);
        }

        public override void Hide()
        {
            base.Hide();
            isSettingPanelActive = false;
            // add inactive style
            settingPanel.AddToClassList(MainMenuUIManager.MODAL_PANEL_INACTIVE_CLASS_NAME);
            settingPanel.RemoveFromClassList(MainMenuUIManager.MODAL_PANEL_ACTIVE_CLASS_NAME);
        }

        private void OnGameDataUpdated()
        {
            soundEffectSlider.value = GameManager.Instance.GameData.SoundEffectsEnabled;
            musicSlider.value = GameManager.Instance.GameData.MusicEnabled;
        }

        protected override void RegisterButtonCallbacks()
        {
            soundEffectSlider?.RegisterCallback<ChangeEvent<bool>>(ChangeSoundEffectToggle);
            musicSlider?.RegisterCallback<ChangeEvent<bool>>(ChangeMusicVolume);
            rateUsButton?.RegisterCallback<ClickEvent>(ClickRateUsButton);
            supportButton?.RegisterCallback<ClickEvent>(ClickSupportButton);
            logoutButton?.RegisterCallback<ClickEvent>(ClickLogoutButton);
        }

        protected override void UnregisterButtonCallbacks()
        {
            soundEffectSlider?.UnregisterCallback<ChangeEvent<bool>>(ChangeSoundEffectToggle);
            musicSlider?.UnregisterCallback<ChangeEvent<bool>>(ChangeMusicVolume);
            rateUsButton?.UnregisterCallback<ClickEvent>(ClickRateUsButton);
            supportButton?.UnregisterCallback<ClickEvent>(ClickSupportButton);
            logoutButton?.UnregisterCallback<ClickEvent>(ClickLogoutButton);
        }

        private void ChangeSoundEffectToggle(ChangeEvent<bool> evt)
        {
            // Check if the event is triggered by the user click
            if (isSettingPanelActive)
            {
                AudioManager.Instance.PlayDefaultButtonSound();
            }

            GameManager.Instance.GameData.SoundEffectsEnabled = evt.newValue;
            AudioManager.Instance.ToggleSoundEffectsMute(evt.newValue);
        }

        private void ChangeMusicVolume(ChangeEvent<bool> evt)
        {
            // Check if the event is triggered by the user clicking the toggle or by the code
            if (isSettingPanelActive)
            {
                AudioManager.Instance.PlayDefaultButtonSound();
            }

            GameManager.Instance.GameData.MusicEnabled = evt.newValue;
            AudioManager.Instance.ToggleMusicMute(evt.newValue);
        }

        private void ClickRateUsButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            Application.OpenURL(GameConstants.RateUsURL);
        }

        private void ClickSupportButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            Application.OpenURL(GameConstants.SupportURL);
        }

        private void ClickLogoutButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            GameManager.Instance.Logout();
        }
    }
}
