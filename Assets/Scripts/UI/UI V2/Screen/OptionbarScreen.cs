using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class OptionbarScreen : Screen
    {
        private const string HomeButtonName = "optionbar__home-button";
        private const string SettingsButtonName = "optionbar__settings-button";
        private const string InventoryButtonName = "optionbar__inventory-button";
        private const string ShopButtonName = "optionbar__shop-button";
        private const string CoinCountName = "optionbar__coin-count";
        private const string GemCountName = "optionbar__gem-count";
        private const string ProfileButtonName = "optionbar__profile-button";
        private const string ProfileNameName = "optionbar__profile-name";
        private const string ProfileImageName = "optionbar__profile-image";
        private const string ProfileRankProgressName = "optionbar__profile-rank-progress";

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
            homeButton = root.Q<Button>(HomeButtonName);
            settingsButton = root.Q<Button>(SettingsButtonName);
            inventoryButton = root.Q<Button>(InventoryButtonName);
            shopButton = root.Q<Button>(ShopButtonName);
            coinCount = root.Q<Label>(CoinCountName);
            gemCount = root.Q<Label>(GemCountName);
            profileButton = root.Q<Button>(ProfileButtonName);
            profileName = root.Q<Label>(ProfileNameName);
            profileImage = root.Q<VisualElement>(ProfileImageName);
            profileRankProgress = root.Q<ProgressBar>(ProfileRankProgressName);
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

        private void OnEnable()
        {
            GameManager.FundsUpdated += OnFundsUpdated;
            GameManager.PlayerDataChanged += OnPlayerDataChanged;
        }

        private void OnDisable()
        {
            GameManager.FundsUpdated -= OnFundsUpdated;
            GameManager.PlayerDataChanged -= OnPlayerDataChanged;
        }

        private void ClickHomeButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowHomeScreen();
        }

        private void ClickSettingsButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowSettingsScreen();
        }

        private void ClickInventoryButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowInventoryScreen();
        }

        private void ClickShopButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            MainMenuUIManager.Instance.ShowShopScreen();
        }

        private void ClickProfileButton(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
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

        void OnFundsUpdated(GameData gameData)
        {
            SetCoin(gameData.Coins);
            SetGems(gameData.Gems);
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
            SetPlayerName(GameManager.Instance.GameData.PlayerDisplayName);
            // SetPlayerProfileImage(GameManager.Instance.GameData.PlayerIcon);
            SetPlayerProgress(GameManager.Instance.GameData.PlayerTrophies);
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