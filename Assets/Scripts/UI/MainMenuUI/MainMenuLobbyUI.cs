using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_playerNameText;
    // [SerializeField] private TextMeshProUGUI m_playerLevelText;
    // [SerializeField] private TextMeshProUGUI m_playerXPText;
    [SerializeField] private TextMeshProUGUI m_playerCurrencyText;
    [SerializeField] private TextMeshProUGUI m_playerGemText;
    [SerializeField] private TextMeshProUGUI m_playerTrophiesText;
    [SerializeField] private Slider m_playerTrophiesSlider;

    void Start()
    {
        GameDataSource.Instance.OnPlayerDataChanged += OnPlayerDataChanged;
    }

    private void OnPlayerDataChanged(object sender, EventArgs e)
    {
        UpdatePlayerData(GameDataSource.Instance.GetPlayerData());
    }

    private void UpdatePlayerData(PlayerDataInventory playerData)
    {
        m_playerNameText.text = playerData.PlayerName;
        // m_playerXPText.text = playerData.Experience.ToString();
        m_playerTrophiesText.text = playerData.Trophies.ToString();
        m_playerTrophiesSlider.value = playerData.Trophies;
        m_playerCurrencyText.text = playerData.Coins.ToString();
        m_playerGemText.text = playerData.Gems.ToString();
    }
}
