using System;
using Managers;
using UI.Base;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class PlayerNamePopupScreen : BaseScreen
    {
        public static event Action<string> PlayerNameSet;
        private const string SET_PLAYER_NAME_BUTTON_NAME = "set-player-name__button";
        private const string PLAYER_NAME_INPUT_FIELD_NAME = "set-player-name__input-field";
        private const string POPUP_PANEL_NAME = "set-player-name__popup-panel";
        private Button setPlayerNameButton;
        private TextField playerNameInputField;
        private VisualElement playerNamePopupPanel;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            setPlayerNameButton = Root.Q<Button>(SET_PLAYER_NAME_BUTTON_NAME);
            playerNameInputField = Root.Q<TextField>(PLAYER_NAME_INPUT_FIELD_NAME);
            playerNamePopupPanel = Root.Q<VisualElement>(POPUP_PANEL_NAME);
        }

        public override void Show()
        {
            base.Show();

            // add active style
            playerNamePopupPanel.RemoveFromClassList(MainMenuUIManager.PopupPanelInactiveClassName);
            playerNamePopupPanel.AddToClassList(MainMenuUIManager.PopupPanelActiveClassName);
        }

        public override void Hide()
        {
            base.Hide();

            // add inactive style
            playerNamePopupPanel.RemoveFromClassList(MainMenuUIManager.PopupPanelActiveClassName);
            playerNamePopupPanel.AddToClassList(MainMenuUIManager.PopupPanelInactiveClassName);
        }

        protected override void RegisterButtonCallbacks()
        {
            setPlayerNameButton?.RegisterCallback<ClickEvent>(ClickSetPlayerNameButton);
        }

        private void ClickSetPlayerNameButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            string playerName = playerNameInputField.text;
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.Log("Player name is empty");
            }
            else
            {
                PlayerNameSet?.Invoke(playerName);
                MainMenuUIManager.Instance.HidePlayerNamePopupScreen();
            }

        }
    }
}