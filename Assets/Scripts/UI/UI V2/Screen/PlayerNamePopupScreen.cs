using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class PlayerNamePopupScreen : Screen
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
            setPlayerNameButton = root.Q<Button>(SET_PLAYER_NAME_BUTTON_NAME);
            playerNameInputField = root.Q<TextField>(PLAYER_NAME_INPUT_FIELD_NAME);
            playerNamePopupPanel = root.Q<VisualElement>(POPUP_PANEL_NAME);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();

            // add active style
            playerNamePopupPanel.RemoveFromClassList(MainMenuUIManager.POPUP_PANEL_INACTIVE_CLASS_NAME);
            playerNamePopupPanel.AddToClassList(MainMenuUIManager.POPUP_PANEL_ACTIVE_CLASS_NAME);
        }

        public override void HideScreen()
        {
            base.HideScreen();

            // add inactive style
            playerNamePopupPanel.RemoveFromClassList(MainMenuUIManager.POPUP_PANEL_ACTIVE_CLASS_NAME);
            playerNamePopupPanel.AddToClassList(MainMenuUIManager.POPUP_PANEL_INACTIVE_CLASS_NAME);
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
                HideScreen();
            }

        }
    }
}