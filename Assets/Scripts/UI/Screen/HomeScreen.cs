using System;
using KitchenKrapper;
using Managers;
using Multiplayer.EOS;
using PlayEveryWare.EpicOnlineServices.Samples;
using UI.Base;
using UI.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Screen
{
    public class HomeScreen : BaseScreen
    {
        public static event Action PlayButtonClicked;
        public static event Action HomeScreenShown;
        private const string HomePanelScreenName = "home-panel";
        private const string PlayLevelButtonName = "home-play__level-button";
        private const string LevelLabelName = "home-play__level-name";
        private const string LevelThumbnailName = "home-play__level-panel";
        private const string LevelSelectButtonName = "home-play__level-select-button";
        private const string InviteCodeLabelName = "home-play__invite-code-label";
        private const string InviteFriendsButtonName = "home-play__invite-friends-button";
        private const string LeaveLobbyButtonName = "home-play__leave-lobby-button";
        private const string LobbyPanelName = "home-play__lobby-panel";
        private const string JoinLobbyButtonName = "home-play__join-lobby-button";
        private const string CreateJoinLobbyContainerName = "home-play__create-join-lobby-container";
        private VisualElement _homePanelScreen;
        private Button _playLevelButton;
        private Button _levelSelectButton;
        private VisualElement _levelThumbnail;
        private Label _levelLabel;
        private Label _inviteCodeLabel;
        private Button _inviteFriendsButton;
        private Button _leaveLobbyButton;
        private Button _joinLobbyButton;
        private VisualElement _createJoinLobbyContainer;

        private void Start()
        {
            HomeScreenController.OnShowLevelInfo += OnShowLevelInfo;
            LobbyManager.Instance.OnLobbyCreated += OnLobbyCreated;
            LobbyManager.Instance.OnLobbyJoined += OnLobbyJoined;
            LobbyManager.Instance.OnLobbyLeft += OnLobbyLeft;
            LobbyManager.Instance.OnLobbyCreatedFailed += OnLobbyCreatedFailed;
            LobbyManager.Instance.OnLobbyJoinedFailed += OnLobbyJoinedFailed;
            LobbyManager.Instance.OnLobbyUpdated += OnLobbyUpdated;
            JoinPopupScreen.JoinPopupScreenHidden += OnJoinPopupScreenHidden;
            OnLobbyLeft();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            _homePanelScreen = Root.Q<VisualElement>(HomePanelScreenName);
            _playLevelButton = Root.Q<Button>(PlayLevelButtonName);
            _levelSelectButton = Root.Q<Button>(LevelSelectButtonName);
            _levelLabel = Root.Q<Label>(LevelLabelName);
            _levelThumbnail = Root.Q(LevelThumbnailName);
            _inviteCodeLabel = Root.Q<Label>(InviteCodeLabelName);
            _inviteFriendsButton = Root.Q<Button>(InviteFriendsButtonName);
            _leaveLobbyButton = Root.Q<Button>(LeaveLobbyButtonName);
            _joinLobbyButton = Root.Q<Button>(JoinLobbyButtonName);
            _createJoinLobbyContainer = Root.Q<VisualElement>(CreateJoinLobbyContainerName);
        }

        protected void OnDisable()
        {
            HomeScreenController.OnShowLevelInfo -= OnShowLevelInfo;
            LobbyManager.Instance.OnLobbyCreated -= OnLobbyCreated;
            LobbyManager.Instance.OnLobbyJoined -= OnLobbyJoined;
            LobbyManager.Instance.OnLobbyLeft -= OnLobbyLeft;
            LobbyManager.Instance.OnLobbyCreatedFailed -= OnLobbyCreatedFailed;
            LobbyManager.Instance.OnLobbyJoinedFailed -= OnLobbyJoinedFailed;
            LobbyManager.Instance.OnLobbyUpdated -= OnLobbyUpdated;
        }

        protected override void RegisterButtonCallbacks()
        {
            _playLevelButton?.RegisterCallback<ClickEvent>(ClickPlayButton);
            _levelSelectButton?.RegisterCallback<ClickEvent>(ClickLevelSelectButton);
            _inviteFriendsButton?.RegisterCallback<ClickEvent>(ClickInviteFriendsButton);
            _leaveLobbyButton?.RegisterCallback<ClickEvent>(ClickLeaveLobbyButton);
            _joinLobbyButton?.RegisterCallback<ClickEvent>(ClickJoinLobbyButton);
        }

        private void OnLobbyCreated(Lobby lobby)
        {
            _inviteCodeLabel.text = GetFormattedInviteCode(lobby.BucketId);
            _inviteCodeLabel.style.display = DisplayStyle.Flex;
            _leaveLobbyButton.style.display = DisplayStyle.Flex;
            _createJoinLobbyContainer.style.display = DisplayStyle.None;
            _leaveLobbyButton.SetEnabled(true);
        }

        private void OnLobbyJoined(Lobby lobby)
        {
            _inviteCodeLabel.text = GetFormattedInviteCode(lobby?.BucketId);
            _inviteCodeLabel.style.display = DisplayStyle.Flex;
            _leaveLobbyButton.style.display = DisplayStyle.Flex;
            _createJoinLobbyContainer.style.display = DisplayStyle.None;
            _leaveLobbyButton.SetEnabled(true);
        }

        private void OnLobbyLeft()
        {
            _inviteCodeLabel.style.display = DisplayStyle.None;
            _leaveLobbyButton.style.display = DisplayStyle.None;
            _createJoinLobbyContainer.style.display = DisplayStyle.Flex;
            _inviteFriendsButton.SetEnabled(true);
            _joinLobbyButton.SetEnabled(true);
        }

        private void OnLobbyCreatedFailed()
        {
            _inviteCodeLabel.style.display = DisplayStyle.None;
            _leaveLobbyButton.style.display = DisplayStyle.None;
            _createJoinLobbyContainer.style.display = DisplayStyle.Flex;
            _inviteFriendsButton.SetEnabled(true);
        }

        private void OnLobbyJoinedFailed()
        {
            _inviteCodeLabel.style.display = DisplayStyle.None;
            _leaveLobbyButton.style.display = DisplayStyle.None;
            _createJoinLobbyContainer.style.display = DisplayStyle.Flex;
            _joinLobbyButton.SetEnabled(true);
        }

        private void OnLobbyUpdated(Lobby lobby)
        {
            _inviteCodeLabel.text = lobby?.BucketId;
        }

        private void OnJoinPopupScreenHidden()
        {
            _joinLobbyButton.SetEnabled(true);
        }

        private string GetFormattedInviteCode(string inviteCode)
        {
            return string.Format("Invite Code: {0}", inviteCode);
        }

        private void ClickLeaveLobbyButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            LobbyManager.Instance.LeaveLobby();
            _leaveLobbyButton.SetEnabled(false);
        }

        private void ClickInviteFriendsButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            LobbyManager.Instance.CreateLobby();
            _inviteFriendsButton.SetEnabled(false);

            // TODO: show invite friends screen
        }

        private static void ClickPlayButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            PlayButtonClicked?.Invoke();
        }

        private static void ClickLevelSelectButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            // MainMenuUIManager.Instance.ShowLevelSelectScreen();
        }

        private void ClickJoinLobbyButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowJoinPopupScreen();
            _joinLobbyButton.SetEnabled(false);
        }

        public override void Show()
        {
            base.Show();
            HomeScreenShown?.Invoke();

            // add active style
            _homePanelScreen.AddToClassList(MainMenuUIManager.ModalPanelActiveClassName);
            _homePanelScreen.RemoveFromClassList(MainMenuUIManager.ModalPanelInactiveClassName);
        }

        public override void Hide()
        {
            base.Hide();

            // add inactive style
            _homePanelScreen.AddToClassList(MainMenuUIManager.ModalPanelInactiveClassName);
            _homePanelScreen.RemoveFromClassList(MainMenuUIManager.ModalPanelActiveClassName);
        }

        // Shows the level information
        private void ShowLevelInfo(string levelName, Sprite thumbnail)
        {
            _levelLabel.text = levelName;
            _levelThumbnail.style.backgroundImage = new StyleBackground(thumbnail);
        }

        // Event-handling methods
        private void OnShowLevelInfo(LevelSO levelData)
        {
            if (levelData == null) return;
            ShowLevelInfo(levelData.levelLabel, levelData.thumbnail);
        }
    }
}