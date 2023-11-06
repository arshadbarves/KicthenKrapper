using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class OptionbarScreen : BaseScreen
    {
        private const string HOME_BUTTON_NAME = "optionbar__home-button";
        private const string SETTINGS_BUTTON_NAME = "optionbar__settings-button";
        private const string INVENTORY_BUTTON_NAME = "optionbar__inventory-button";
        private const string SHOP_BUTTON_NAME = "optionbar__shop-button";
        private const string COIN_COUNT_LABEL_NAME = "optionbar__coin-count";
        private const string GEM_COUNT_LABEL_NAME = "optionbar__gem-count";
        private const string PROFILE_BUTTON_NAME = "optionbar__profile-button";
        private const string PROFILE_NAME_LABEL_NAME = "optionbar__profile-name";
        private const string PROFILE_IMAGE_NAME = "optionbar__profile-image";
        private const string PROFILE_RANK_PROGRESS_NAME = "optionbar__profile-rank-progress";

        private Button homeButton;
        private Button settingsButton;
        private Button inventoryButton;
        private Button shopButton;
        private Label coinCount;
        private Label gemCount;
        private Button profileButton;
        private Label profileName;
        private VisualElement profileImage;
        private ProgressBar profileRankProgress;

        private const float LerpTime = 0.6f;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            homeButton = root.Q<Button>(HOME_BUTTON_NAME);
            settingsButton = root.Q<Button>(SETTINGS_BUTTON_NAME);
            inventoryButton = root.Q<Button>(INVENTORY_BUTTON_NAME);
            shopButton = root.Q<Button>(SHOP_BUTTON_NAME);
            coinCount = root.Q<Label>(COIN_COUNT_LABEL_NAME);
            gemCount = root.Q<Label>(GEM_COUNT_LABEL_NAME);
            profileButton = root.Q<Button>(PROFILE_BUTTON_NAME);
            profileName = root.Q<Label>(PROFILE_NAME_LABEL_NAME);
            profileImage = root.Q<VisualElement>(PROFILE_IMAGE_NAME);
            profileRankProgress = root.Q<ProgressBar>(PROFILE_RANK_PROGRESS_NAME);
        }

        protected override void RegisterButtonCallbacks()
        {
            homeButton?.RegisterCallback<ClickEvent>(ClickHomeButton);
            settingsButton?.RegisterCallback<ClickEvent>(ClickSettingsButton);
            inventoryButton?.RegisterCallback<ClickEvent>(ClickInventoryButton);
            shopButton?.RegisterCallback<ClickEvent>(ClickShopButton);
            profileButton?.RegisterCallback<ClickEvent>(ClickProfileButton);
        }

        protected override void UnregisterButtonCallbacks()
        {
            homeButton?.UnregisterCallback<ClickEvent>(ClickHomeButton);
            settingsButton?.UnregisterCallback<ClickEvent>(ClickSettingsButton);
            inventoryButton?.UnregisterCallback<ClickEvent>(ClickInventoryButton);
            shopButton?.UnregisterCallback<ClickEvent>(ClickShopButton);
            profileButton?.UnregisterCallback<ClickEvent>(ClickProfileButton);
        }

        private void Start()
        {
            GameManager.PlayerDataChanged += OnPlayerDataChanged;
        }

        protected override void OnDestroy()
        {
            GameManager.PlayerDataChanged -= OnPlayerDataChanged;
        }

        private void ClickHomeButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowHomeScreen();
        }

        private void ClickSettingsButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowSettingsScreen();
        }

        private void ClickInventoryButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowInventoryScreen();
        }

        private void ClickShopButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowShopScreen();
        }

        private void ClickProfileButton(ClickEvent evt)
        {
            AudioManager.Instance.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowPlayerNamePopupScreen();
        }

        public void SetCoin(uint coins)
        {
            uint startValue = (uint)Int32.Parse(coinCount.text);
            StartCoroutine(LerpRoutine(coinCount, startValue, coins, LerpTime));
        }

        public void SetGems(uint gems)
        {
            uint startValue = (uint)Int32.Parse(gemCount.text);
            StartCoroutine(LerpRoutine(gemCount, startValue, gems, LerpTime));
        }

        public void SetPlayerName(string playerName)
        {
            profileName.text = playerName;
        }

        public void SetPlayerProfileImage(string profileImageName)
        {
            Sprite sprite = Resources.Load<Sprite>("Images/Profile/" + profileImageName);
            profileImage.style.backgroundImage = sprite.texture;
        }

        public void SetPlayerProgress(uint playerTrophies)
        {
            uint startValue = (uint)profileRankProgress.value;
            StartCoroutine(LerpProgressRoutine(profileRankProgress, startValue, playerTrophies, LerpTime));
        }

        void OnPlayerDataChanged()
        {

            SetCoin(GameManager.Instance.PlayerData.Coins);
            SetGems(GameManager.Instance.PlayerData.Gems);
            SetPlayerName(GameManager.Instance.PlayerData.PlayerDisplayName);
            print("[Option]" + GameManager.Instance.PlayerData.ToJson());
            SetPlayerProgress(GameManager.Instance.PlayerData.PlayerTrophies);
        }

        IEnumerator LerpProgressRoutine(ProgressBar progressBar, float startValue, float endValue, float duration)
        {
            float lerpValue = startValue;
            float t = 0f;
            progressBar.value = 0f;

            while (Mathf.Abs(lerpValue - endValue) > 0.01f)
            {
                t += Time.deltaTime / LerpTime;

                lerpValue = Mathf.Lerp(startValue, endValue, t);
                progressBar.value = lerpValue;
                yield return null;
            }
            progressBar.value = endValue;
        }

        // animated Label counter
        IEnumerator LerpRoutine(Label label, uint startValue, uint endValue, float duration)
        {
            float lerpValue = startValue;
            float t = 0f;
            label.text = string.Empty;

            while (Mathf.Abs(lerpValue - endValue) > 0.01f)
            {
                t += Time.deltaTime / LerpTime;

                lerpValue = Mathf.Lerp(startValue, endValue, t);
                label.text = lerpValue.ToString("0");
                yield return null;
            }
            label.text = endValue.ToString();
        }
    }
}