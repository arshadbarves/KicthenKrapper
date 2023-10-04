using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class EULAScreen : Screen
    {
        private const string TermsOfUseButtonName = "eula-privacy__terms-of-use-button";
        private const string PrivacyPolicyButtonName = "eula-privacy__privacy-policy-button";
        private const string AcceptEULAButtonName = "eula-privacy__accept-eula-button";
        private const string DeclineEULAButtonName = "eula-privacy__decline-eula-button";
        private const string PopupPanelName = "eula-privacy__popup-panel";
        private Button termsOfUseButton;
        private Button privacyPolicyButton;
        private Button acceptEULAButton;
        private Button declineEULAButton;

        private VisualElement eulaPopupPanel;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            termsOfUseButton = root.Q<Button>(TermsOfUseButtonName);
            privacyPolicyButton = root.Q<Button>(PrivacyPolicyButtonName);
            acceptEULAButton = root.Q<Button>(AcceptEULAButtonName);
            declineEULAButton = root.Q<Button>(DeclineEULAButtonName);
            eulaPopupPanel = root.Q<VisualElement>(PopupPanelName);
        }

        protected override void RegisterButtonCallbacks()
        {
            termsOfUseButton?.RegisterCallback<ClickEvent>(ClickTermsOfUseButton);
            privacyPolicyButton?.RegisterCallback<ClickEvent>(ClickPrivacyPolicyButton);
            acceptEULAButton?.RegisterCallback<ClickEvent>(ClickAcceptEULAButton);
            declineEULAButton?.RegisterCallback<ClickEvent>(ClickDeclineEULAButton);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();

            // add active style
            eulaPopupPanel.RemoveFromClassList(MainMenuUIManager.PopupPanelInactiveClassName);
            eulaPopupPanel.AddToClassList(MainMenuUIManager.PopupPanelActiveClassName);
        }

        public override void HideScreen()
        {
            base.HideScreen();

            // add inactive style
            eulaPopupPanel.RemoveFromClassList(MainMenuUIManager.PopupPanelActiveClassName);
            eulaPopupPanel.AddToClassList(MainMenuUIManager.PopupPanelInactiveClassName);
        }

        private void ClickTermsOfUseButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            Application.OpenURL(GameConstants.TermsOfUseUrl);
        }

        private void ClickPrivacyPolicyButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            Application.OpenURL(GameConstants.PrivacyPolicyUrl);
        }

        private void ClickAcceptEULAButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            HideScreen();
            ClientPrefs.SetEULAAndPrivacyPolicyAccepted(true);
            GameManager.Instance.StartApp();
        }

        private void ClickDeclineEULAButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            HideScreen();
            GameManager.Instance.QuitGame();
        }
    }
}