using System;
using Managers;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class JoinPopupScreen : BaseScreen
    {
        public static event Action JoinPopupScreenHidden;

        private const string JOIN_PANEL_POPUP_NAME = "home-play__join-panel__popup";
        private const string JOIN_PANEL_POPUP_CLOSE_BUTTON_NAME = "home-play__join-panel__popup__close-button";
        private const string JOIN_PANEL_INVITE_CODE_FIELD_NAME = "home-play__join-panel__invite-code-field";
        private const string JOIN_PANEL_JOIN_BUTTON_NAME = "home-play__join-panel__join-button";

        private VisualElement joinPanelPopup;
        private Button joinPanelPopupCloseButton;
        private TextField joinPanelInviteCodeField;
        private Button joinPanelJoinButton;

        private void Start()
        {
            LobbyManager.Instance.LobbyJoinedFailed += OnLobbyJoinedFailed;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            joinPanelPopup = root.Q<VisualElement>(JOIN_PANEL_POPUP_NAME);
            joinPanelPopupCloseButton = root.Q<Button>(JOIN_PANEL_POPUP_CLOSE_BUTTON_NAME);
            joinPanelInviteCodeField = root.Q<TextField>(JOIN_PANEL_INVITE_CODE_FIELD_NAME);
            joinPanelJoinButton = root.Q<Button>(JOIN_PANEL_JOIN_BUTTON_NAME);
        }

        protected override void RegisterButtonCallbacks()
        {
            joinPanelPopupCloseButton?.RegisterCallback<ClickEvent>(ClickJoinPanelPopupCloseButton);
            joinPanelJoinButton?.RegisterCallback<ClickEvent>(ClickJoinPanelJoinButton);
        }

        protected override void UnregisterButtonCallbacks()
        {
            joinPanelPopupCloseButton?.UnregisterCallback<ClickEvent>(ClickJoinPanelPopupCloseButton);
            joinPanelJoinButton?.UnregisterCallback<ClickEvent>(ClickJoinPanelJoinButton);
        }

        private void ClickJoinPanelPopupCloseButton(ClickEvent evt)
        {
            MainMenuUIManager.Instance.HideJoinPopupScreen();
        }

        private void ClickJoinPanelJoinButton(ClickEvent evt)
        {
            joinPanelJoinButton.SetEnabled(false);
            string inviteCode = joinPanelInviteCodeField.text;
            if (inviteCode.Length > 0)
            {
                LobbyManager.Instance.FindAndJoinLobby(inviteCode);
            }
            else
            {
                joinPanelJoinButton.SetEnabled(true);
            }
        }

        public override void Show()
        {
            base.Show();

            // add active style
            joinPanelPopup.RemoveFromClassList(MainMenuUIManager.POPUP_PANEL_INACTIVE_CLASS_NAME);
            joinPanelPopup.AddToClassList(MainMenuUIManager.POPUP_PANEL_ACTIVE_CLASS_NAME);
        }

        public override void Hide()
        {
            base.Hide();

            // add inactive style
            // joinPanelPopup.RemoveFromClassList(MainMenuUIManager.POPUP_PANEL_ACTIVE_CLASS_NAME);
            // joinPanelPopup.AddToClassList(MainMenuUIManager.POPUP_PANEL_INACTIVE_CLASS_NAME);

            JoinPopupScreenHidden?.Invoke();
        }

        private void OnLobbyJoinedFailed()
        {
            joinPanelJoinButton.SetEnabled(true);
        }
    }
}