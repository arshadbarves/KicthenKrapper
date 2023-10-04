using MyUILibrary;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class SettingsScreen : Screen
    {
        private const string SettingPanelName = "settings__panel";
        private const string SoundEffectSliderName = "sound-effect__slider";
        private const string MusicSliderName = "music__slider";
        private const string RateUsButtonName = "rate-us__button";
        private const string SupportButtonName = "support__button";
        private const string LogoutButtonName = "logout__button";

        private VisualElement settingPanel;
        private SlideToggle soundEffectSlider;
        private SlideToggle musicSlider;
        private Button rateUsButton;
        private Button supportButton;
        private Button logoutButton;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            settingPanel = root.Q<VisualElement>(SettingPanelName);
            soundEffectSlider = root.Q<SlideToggle>(SoundEffectSliderName);
            musicSlider = root.Q<SlideToggle>(MusicSliderName);
            rateUsButton = root.Q<Button>(RateUsButtonName);
            supportButton = root.Q<Button>(SupportButtonName);
            logoutButton = root.Q<Button>(LogoutButtonName);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();

            soundEffectSlider.value = AudioManager.Instance.AreSoundEffectsMuted();
            musicSlider.value = AudioManager.Instance.IsMusicMuted();

            // add active style
            settingPanel.AddToClassList(MainMenuUIManager.ModalPanelActiveClassName);
            settingPanel.RemoveFromClassList(MainMenuUIManager.ModalPanelInactiveClassName);
        }

        public override void HideScreen()
        {
            base.HideScreen();

            // add inactive style
            settingPanel.AddToClassList(MainMenuUIManager.ModalPanelInactiveClassName);
            settingPanel.RemoveFromClassList(MainMenuUIManager.ModalPanelActiveClassName);
        }

        protected override void RegisterButtonCallbacks()
        {
            soundEffectSlider?.RegisterCallback<ChangeEvent<bool>>(ChangeSoundEffectToggle);
            musicSlider?.RegisterCallback<ChangeEvent<bool>>(ChangeMusicVolume);
            rateUsButton?.RegisterCallback<ClickEvent>(ClickRateUsButton);
            supportButton?.RegisterCallback<ClickEvent>(ClickSupportButton);
            logoutButton?.RegisterCallback<ClickEvent>(ClickLogoutButton);
        }

        private void ChangeSoundEffectToggle(ChangeEvent<bool> evt)
        {
            AudioManager.PlayDefaultButtonSound();
            AudioManager.Instance.ToggleSoundEffectsMute(evt.newValue);
        }

        private void ChangeMusicVolume(ChangeEvent<bool> evt)
        {
            AudioManager.PlayDefaultButtonSound();
            AudioManager.Instance.ToggleMusicMute(evt.newValue);
        }

        private void ClickRateUsButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            Application.OpenURL(GameConstants.RateUsURL);
        }

        private void ClickSupportButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            Application.OpenURL(GameConstants.SupportURL);
        }

        private void ClickLogoutButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            GameManager.Instance.Logout();
        }
    }
}
