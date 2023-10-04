using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class PlayerNamePopupScreen : Screen
    {
        private const string SetPlayerNameButtonName = "set-player-name__button";
        private const string PlayerNameInputFieldName = "set-player-name__input-field";
        private const string PopupPanelName = "set-player-name__popup-panel";
        private Button setPlayerNameButton;
        private TextField playerNameInputField;
        private VisualElement playerNamePopupPanel;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            setPlayerNameButton = root.Q<Button>(SetPlayerNameButtonName);
            playerNameInputField = root.Q<TextField>(PlayerNameInputFieldName);
            playerNamePopupPanel = root.Q<VisualElement>(PopupPanelName);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();

            // add active style
            playerNamePopupPanel.RemoveFromClassList(MainMenuUIManager.PopupPanelInactiveClassName);
            playerNamePopupPanel.AddToClassList(MainMenuUIManager.PopupPanelActiveClassName);
        }

        public override void HideScreen()
        {
            base.HideScreen();

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
            AudioManager.PlayDefaultButtonSound();
            string playerName = playerNameInputField.text;
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.Log("Player name is empty");
            }
            else
            {
                GameManager.Instance.CreatePlayerData(playerName);
                HideScreen();
            }

        }
    }
}