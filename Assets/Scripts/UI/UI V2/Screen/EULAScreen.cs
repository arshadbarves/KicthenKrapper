using System;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class EULAScreen : BaseScreen
    {
        public static event Action EULAAccepted;
        private const string TERMS_OF_USE_BUTTON_NAME = "eula-privacy__terms-of-use-button";
        private const string PRIVACY_POLICY_BUTTON_NAME = "eula-privacy__privacy-policy-button";
        private const string ACCEPT_EULA_BUTTON_NAME = "eula-privacy__accept-eula-button";
        private const string DECLLINE_EULA_BUTTON_NAME = "eula-privacy__decline-eula-button";
        private const string POPUP_PANEL_NAME = "eula-privacy__popup-panel";
        private Button termsOfUseButton;
        private Button privacyPolicyButton;
        private Button acceptEULAButton;
        private Button declineEULAButton;

        private VisualElement eulaPopupPanel;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            termsOfUseButton = root.Q<Button>(TERMS_OF_USE_BUTTON_NAME);
            privacyPolicyButton = root.Q<Button>(PRIVACY_POLICY_BUTTON_NAME);
            acceptEULAButton = root.Q<Button>(ACCEPT_EULA_BUTTON_NAME);
            declineEULAButton = root.Q<Button>(DECLLINE_EULA_BUTTON_NAME);
            eulaPopupPanel = root.Q<VisualElement>(POPUP_PANEL_NAME);
        }

        protected override void RegisterButtonCallbacks()
        {
            termsOfUseButton?.RegisterCallback<ClickEvent>(ClickTermsOfUseButton);
            privacyPolicyButton?.RegisterCallback<ClickEvent>(ClickPrivacyPolicyButton);
            acceptEULAButton?.RegisterCallback<ClickEvent>(ClickAcceptEULAButton);
            declineEULAButton?.RegisterCallback<ClickEvent>(ClickDeclineEULAButton);
        }

        public override void Show()
        {
            base.Show();

            // add active style
            eulaPopupPanel.RemoveFromClassList(MainMenuUIManager.POPUP_PANEL_INACTIVE_CLASS_NAME);
            eulaPopupPanel.AddToClassList(MainMenuUIManager.POPUP_PANEL_ACTIVE_CLASS_NAME);
        }

        public override void Hide()
        {
            base.Hide();

            // add inactive style
            eulaPopupPanel.RemoveFromClassList(MainMenuUIManager.POPUP_PANEL_ACTIVE_CLASS_NAME);
            eulaPopupPanel.AddToClassList(MainMenuUIManager.POPUP_PANEL_INACTIVE_CLASS_NAME);
        }

        private void ClickTermsOfUseButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            Application.OpenURL(GameConstants.TermsOfUseUrl);
        }

        private void ClickPrivacyPolicyButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            Application.OpenURL(GameConstants.PrivacyPolicyUrl);
        }

        private void ClickAcceptEULAButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.HideEULAScreen();
            EULAAccepted?.Invoke();
        }

        private void ClickDeclineEULAButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.HideEULAScreen();
            GameManager.Instance.QuitGame();
        }
    }
}