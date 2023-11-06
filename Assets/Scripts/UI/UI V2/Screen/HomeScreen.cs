using UnityEngine;
using UnityEngine.UIElements;
using System;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace KitchenKrapper
{
    public class HomeScreen : BaseScreen
    {
        public static event Action PlayButtonClicked;
        public static event Action HomeScreenShown;

        private const string HOME_PANEL_SCREEN_NAME = "home-panel";
        private const string PLAY_LEVEL_BUTTON_NAME = "home-play__level-button";
        private const string LEVEL_LABEL_NAME = "home-play__level-name";
        private const string LEVEL_THUMBNAIL_NAME = "home-play__level-panel";
        private const string LEVEL_SELECT_BUTTON_NAME = "home-play__level-select-button";
        private const string INVITE_CODE_LABEL_NAME = "home-play__invite-code-label";
        private const string INVITE_FRIENDS_BUTTON_NAME = "home-play__invite-friends-button";
        private const string LEAVE_LOBBY_BUTTON_NAME = "home-play__leave-lobby-button";
        private const string LOBBY_PANEL_NAME = "home-play__lobby-panel";
        private const string JOIN_LOBBY_BUTTON_NAME = "home-play__join-lobby-button";
        private const string CREATE_JOIN_LOBBY_CONTAINER_NAME = "home-play__create-join-lobby-container";

        private VisualElement homePanelScreen;
        private Button playLevelButton;
        private Button levelSelectButton;
        private VisualElement levelThumbnail;
        private Label levelLabel;
        private Label inviteCodeLabel;
        private Button inviteFriendsButton;
        private Button leaveLobbyButton;
        private Button joinLobbyButton;
        private VisualElement createJoinLobbyContainer;

        private void Start()
        {
            HomeScreenController.ShowLevelInfo += OnShowLevelInfo;
            LobbyManager.Instance.LobbyCreated += OnLobbyCreated;
            LobbyManager.Instance.LobbyJoined += OnLobbyJoined;
            LobbyManager.Instance.LobbyLeft += OnLobbyLeft;
            LobbyManager.Instance.LobbyCreatedFailed += OnLobbyCreatedFailed;
            LobbyManager.Instance.LobbyJoinedFailed += OnLobbyJoinedFailed;
            LobbyManager.Instance.LobbyUpdated += OnLobbyUpdated;
            JoinPopupScreen.JoinPopupScreenHidden += OnJoinPopupScreenHidden;

            OnLobbyLeft();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            homePanelScreen = root.Q<VisualElement>(HOME_PANEL_SCREEN_NAME);
            playLevelButton = root.Q<Button>(PLAY_LEVEL_BUTTON_NAME);
            levelSelectButton = root.Q<Button>(LEVEL_SELECT_BUTTON_NAME);
            levelLabel = root.Q<Label>(LEVEL_LABEL_NAME);
            levelThumbnail = root.Q(LEVEL_THUMBNAIL_NAME);
            inviteCodeLabel = root.Q<Label>(INVITE_CODE_LABEL_NAME);
            inviteFriendsButton = root.Q<Button>(INVITE_FRIENDS_BUTTON_NAME);
            leaveLobbyButton = root.Q<Button>(LEAVE_LOBBY_BUTTON_NAME);
            joinLobbyButton = root.Q<Button>(JOIN_LOBBY_BUTTON_NAME);
            createJoinLobbyContainer = root.Q<VisualElement>(CREATE_JOIN_LOBBY_CONTAINER_NAME);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            HomeScreenController.ShowLevelInfo -= OnShowLevelInfo;
            LobbyManager.Instance.LobbyCreated -= OnLobbyCreated;
            LobbyManager.Instance.LobbyJoined -= OnLobbyJoined;
            LobbyManager.Instance.LobbyLeft -= OnLobbyLeft;
            LobbyManager.Instance.LobbyCreatedFailed -= OnLobbyCreatedFailed;
            LobbyManager.Instance.LobbyJoinedFailed -= OnLobbyJoinedFailed;
            LobbyManager.Instance.LobbyUpdated -= OnLobbyUpdated;
        }

        protected override void RegisterButtonCallbacks()
        {
            playLevelButton?.RegisterCallback<ClickEvent>(ClickPlayButton);
            levelSelectButton?.RegisterCallback<ClickEvent>(ClickLevelSelectButton);
            inviteFriendsButton?.RegisterCallback<ClickEvent>(ClickInviteFriendsButton);
            leaveLobbyButton?.RegisterCallback<ClickEvent>(ClickLeaveLobbyButton);
            joinLobbyButton?.RegisterCallback<ClickEvent>(ClickJoinLobbyButton);
        }

        private void OnLobbyCreated(Lobby lobby)
        {
            inviteCodeLabel.text = GetFormattedInviteCode(lobby.BucketId);
            inviteCodeLabel.style.display = DisplayStyle.Flex;
            leaveLobbyButton.style.display = DisplayStyle.Flex;
            createJoinLobbyContainer.style.display = DisplayStyle.None;

            leaveLobbyButton.SetEnabled(true);
        }

        private void OnLobbyJoined(Lobby lobby)
        {
            inviteCodeLabel.text = GetFormattedInviteCode(lobby?.BucketId);
            inviteCodeLabel.style.display = DisplayStyle.Flex;
            leaveLobbyButton.style.display = DisplayStyle.Flex;
            createJoinLobbyContainer.style.display = DisplayStyle.None;

            leaveLobbyButton.SetEnabled(true);
        }

        private void OnLobbyLeft()
        {
            inviteCodeLabel.style.display = DisplayStyle.None;
            leaveLobbyButton.style.display = DisplayStyle.None;
            createJoinLobbyContainer.style.display = DisplayStyle.Flex;

            inviteFriendsButton.SetEnabled(true);
            joinLobbyButton.SetEnabled(true);
        }

        private void OnLobbyCreatedFailed()
        {
            inviteCodeLabel.style.display = DisplayStyle.None;
            leaveLobbyButton.style.display = DisplayStyle.None;
            createJoinLobbyContainer.style.display = DisplayStyle.Flex;

            inviteFriendsButton.SetEnabled(true);
        }

        private void OnLobbyJoinedFailed()
        {
            inviteCodeLabel.style.display = DisplayStyle.None;
            leaveLobbyButton.style.display = DisplayStyle.None;
            createJoinLobbyContainer.style.display = DisplayStyle.Flex;

            joinLobbyButton.SetEnabled(true);
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            inviteCodeLabel.text = lobby?.BucketId;
        }

        private void OnJoinPopupScreenHidden()
        {
            joinLobbyButton.SetEnabled(true);
        }

        private string GetFormattedInviteCode(string inviteCode)
        {
            return string.Format("Invite Code: {0}", inviteCode);
        }

        private void ClickLeaveLobbyButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            LobbyManager.Instance.LeaveLobby();

            leaveLobbyButton.SetEnabled(false);
        }

        private void ClickInviteFriendsButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            LobbyManager.Instance.CreateLobby();

            inviteFriendsButton.SetEnabled(false);

            // TODO: show invite friends screen
        }

        private void ClickPlayButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            PlayButtonClicked?.Invoke();
        }

        private void ClickLevelSelectButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            // MainMenuUIManager.Instance.ShowLevelSelectScreen();
        }

        private void ClickJoinLobbyButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();

            MainMenuUIManager.Instance.ShowJoinPopupScreen();

            joinLobbyButton.SetEnabled(false);
        }

        public override void Show()
        {
            base.Show();
            HomeScreenShown?.Invoke();

            // add active style
            homePanelScreen.AddToClassList(MainMenuUIManager.MODAL_PANEL_ACTIVE_CLASS_NAME);
            homePanelScreen.RemoveFromClassList(MainMenuUIManager.MODAL_PANEL_INACTIVE_CLASS_NAME);
        }

        public override void Hide()
        {
            base.Hide();

            // add inactive style
            homePanelScreen.AddToClassList(MainMenuUIManager.MODAL_PANEL_INACTIVE_CLASS_NAME);
            homePanelScreen.RemoveFromClassList(MainMenuUIManager.MODAL_PANEL_ACTIVE_CLASS_NAME);
        }

        // Shows the level information
        public void ShowLevelInfo(string levelName, Sprite thumbnail)
        {
            levelLabel.text = levelName;
            levelThumbnail.style.backgroundImage = new StyleBackground(thumbnail);
        }

        // Event-handling methods
        private void OnShowLevelInfo(LevelSO levelData)
        {
            if (levelData == null)
                return;

            ShowLevelInfo(levelData.levelLabel, levelData.thumbnail);
        }
    }
}
