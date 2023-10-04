using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
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

        public static PlayerInventory Instance { get; private set; }

        private PlayerData playerData = new PlayerData();

        private void Awake()
        {
            InitializeSingleton();
            LoadPlayerData();
        }

        private void InitializeSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void LoadPlayerData()
        {
            playerData = SecureDataStorage.LoadData(playerData);

            OnCoinsChanged?.Invoke(this, new CoinsEventArgs { coins = playerData.coins });
            OnGemsChanged?.Invoke(this, new GemsEventArgs { gems = playerData.gems });
            OnSkinChanged?.Invoke(this, new SkinEventArgs { skin = playerData.selectedSkin });
        }

        public int GetCoins() => playerData.coins;
        public int GetGems() => playerData.gems;

        public void AddCoins(int amount)
        {
            playerData.coins += amount;
            OnCoinsChanged?.Invoke(this, new CoinsEventArgs { coins = playerData.coins });
        }

        public void AddGems(int amount)
        {
            playerData.gems += amount;
            OnGemsChanged?.Invoke(this, new GemsEventArgs { gems = playerData.gems });
        }

        public void SpendCoins(int amount)
        {
            playerData.coins = Mathf.Max(0, playerData.coins - amount);
            OnCoinsChanged?.Invoke(this, new CoinsEventArgs { coins = playerData.coins });
        }

        public void SpendGems(int amount)
        {
            playerData.gems = Mathf.Max(0, playerData.gems - amount);
            OnGemsChanged?.Invoke(this, new GemsEventArgs { gems = playerData.gems });
        }

        public bool HasEnoughCoins(int amount) => playerData.coins >= amount;
        public bool HasEnoughGems(int amount) => playerData.gems >= amount;

        public bool HasSkin(SkinSO skin) => playerData.skins.ContainsKey(skin.SkinID);

        public SkinSO GetSelectedSkin() => playerData.selectedSkin;

        public void SetSelectedSkin(SkinSO skin)
        {
            playerData.selectedSkin = skin;
            OnSkinChanged?.Invoke(this, new SkinEventArgs { skin = playerData.selectedSkin });
        }

        public void UnlockSkin(SkinSO skin)
        {
            playerData.skins.Add(skin.SkinID, skin);
        }

        public Dictionary<string, SkinSO> GetUnlockedSkins() => playerData.skins;

        [Serializable]
        public class CoinsEventArgs : EventArgs
        {
            public int coins;
        }

        [Serializable]
        public class GemsEventArgs : EventArgs
        {
            public int gems;
        }

        [Serializable]
        public class SkinEventArgs : EventArgs
        {
            public SkinSO skin;
        }
    }
}