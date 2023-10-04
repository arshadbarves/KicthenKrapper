using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace KitchenKrapper
{
    public class HomeScreen : Screen
    {
        public static event Action PlayButtonClicked;
        public static event Action HomeScreenShown;

        private const string HomePanelScreenName = "home-panel";
        private const string PlayLevelButtonName = "home-play__level-button";
        private const string LevelLabelName = "home-play__level-name";
        private const string LevelThumbnailName = "home-play__level-panel";
        private const string LevelSelectButtonName = "home-play__level-select-button";

        private VisualElement homePanelScreen;
        private Button playLevelButton;
        private Button levelSelectButton;
        private VisualElement levelThumbnail;
        private Label levelLabel;

        private void OnEnable()
        {
            HomeScreenController.ShowLevelInfo += OnShowLevelInfo;
        }

        private void OnDisable()
        {
            HomeScreenController.ShowLevelInfo -= OnShowLevelInfo;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            homePanelScreen = root.Q<VisualElement>(HomePanelScreenName);
            playLevelButton = root.Q<Button>(PlayLevelButtonName);
            levelSelectButton = root.Q<Button>(LevelSelectButtonName);
            levelLabel = root.Q<Label>(LevelLabelName);
            levelThumbnail = root.Q(LevelThumbnailName);
        }

        protected override void RegisterButtonCallbacks()
        {
            playLevelButton?.RegisterCallback<ClickEvent>(ClickPlayButton);
            levelSelectButton?.RegisterCallback<ClickEvent>(ClickLevelSelectButton);
        }

        private void ClickPlayButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            PlayButtonClicked?.Invoke();
        }

        private void ClickLevelSelectButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            // MainMenuUIManager.Instance.ShowLevelSelectScreen();
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            HomeScreenShown?.Invoke();

            // add active style
            homePanelScreen.AddToClassList(MainMenuUIManager.ModalPanelActiveClassName);
            homePanelScreen.RemoveFromClassList(MainMenuUIManager.ModalPanelInactiveClassName);
        }

        public override void HideScreen()
        {
            base.HideScreen();

            // add inactive style
            homePanelScreen.AddToClassList(MainMenuUIManager.ModalPanelInactiveClassName);
            homePanelScreen.RemoveFromClassList(MainMenuUIManager.ModalPanelActiveClassName);
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
