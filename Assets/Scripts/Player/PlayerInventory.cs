using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Serializable]
    public class PlayerData
    {
        public string playerName;
        public int coins;
        public int gems;
        public Dictionary<string, SkinSO> skins;
        public SkinSO selectedSkin;
    }

    public static event EventHandler<CoinsEventArgs> OnCoinsChanged;
    public static event EventHandler<GemsEventArgs> OnGemsChanged;
    public static event EventHandler<SkinEventArgs> OnSkinChanged;

    public class CoinsEventArgs : EventArgs
    {
        public int coins;
    }

    public class GemsEventArgs : EventArgs
    {
        public int gems;
    }

    public class SkinEventArgs : EventArgs
    {
        public SkinSO skin;
    }

    public static PlayerInventory Instance { get; private set; }

    private string playerName = "Player";
    private int coins = 0;
    private int gems = 0;
    private Dictionary<string, SkinSO> skins = new Dictionary<string, SkinSO>();
    public SkinSO selectedSkin;

    private void Awake()
    {
        Instance = this;
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        PlayerData playerData = SecureDataStorage.LoadData<PlayerData>(new PlayerData());
        playerName = playerData.playerName;
        coins = playerData.coins;
        gems = playerData.gems;
        skins = playerData.skins;
        selectedSkin = playerData.selectedSkin;

        OnCoinsChanged?.Invoke(this, new CoinsEventArgs { coins = coins });
        OnGemsChanged?.Invoke(this, new GemsEventArgs { gems = gems });
        OnSkinChanged?.Invoke(this, new SkinEventArgs { skin = selectedSkin });
    }

    public int GetCoins() { return coins; }
    public int GetGems() { return gems; }

    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke(this, new CoinsEventArgs { coins = coins });
    }

    public void AddGems(int amount)
    {
        gems += amount;
        OnGemsChanged?.Invoke(this, new GemsEventArgs { gems = gems });
    }

    public void SpendCoins(int amount)
    {
        if (coins - amount < 0)
        {
            coins = 0;
        }
        else
        {
            coins -= amount;
        }
        OnCoinsChanged?.Invoke(this, new CoinsEventArgs { coins = coins });
    }

    public void SpendGems(int amount)
    {
        if (gems - amount < 0)
        {
            gems = 0;
        }
        else
        {
            gems -= amount;
        }
        OnGemsChanged?.Invoke(this, new GemsEventArgs { gems = gems });
    }

    public bool HasEnoughCoins(int amount)
    {
        return coins >= amount;
    }

    public bool HasEnoughGems(int amount)
    {
        return gems >= amount;
    }

    public bool HasSkin(SkinSO skin)
    {
        return skins.ContainsKey(skin.SkinID);
    }

    public SkinSO GetSelectedSkin()
    {
        return selectedSkin;
    }

    public void SetSelectedSkin(SkinSO skin)
    {
        selectedSkin = skin;
        OnSkinChanged?.Invoke(this, new SkinEventArgs { skin = selectedSkin });
    }

    public void UnlockSkin(SkinSO skin)
    {
        skins.Add(skin.SkinID, skin);
    }

    public Dictionary<string, SkinSO> GetUnlockedSkins()
    {
        return skins;
    }
}
