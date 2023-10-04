using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class MainMenuLobbyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerCurrencyText;
        [SerializeField] private TextMeshProUGUI playerGemText;
        [SerializeField] private TextMeshProUGUI playerTrophiesText;
        [SerializeField] private Slider playerTrophiesSlider;

        private void Start()
        {
            SubscribeToPlayerDataChangedEvent();
        }

        private void OnDestroy()
        {
            UnsubscribeFromPlayerDataChangedEvent();
        }

        private void SubscribeToPlayerDataChangedEvent()
        {
            GameDataSource.Instance.OnPlayerDataChanged += OnPlayerDataChanged;
        }

        private void UnsubscribeFromPlayerDataChangedEvent()
        {
            GameDataSource.Instance.OnPlayerDataChanged -= OnPlayerDataChanged;
        }

        private void OnPlayerDataChanged(object sender, EventArgs e)
        {
            UpdatePlayerData(GameDataSource.Instance.GetPlayerData());
        }

        private void UpdatePlayerData(PlayerDataInventory playerData)
        {
            playerNameText.text = playerData.PlayerName;
            playerTrophiesText.text = playerData.Trophies.ToString();
            playerTrophiesSlider.value = playerData.Trophies;
            playerCurrencyText.text = playerData.Coins.ToString();
            playerGemText.text = playerData.Gems.ToString();
        }
    }
}
