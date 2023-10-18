using System;
using UnityEngine;
using Epic.OnlineServices;

namespace KitchenKrapper
{
    [Serializable]
    public class PlayerGameData
    {
        [SerializeField]
        private uint coins;
        [SerializeField]
        private uint gems;
        [SerializeField]
        private ProductUserId playerId;
        [SerializeField]
        private string playerDisplayName;
        [SerializeField]
        private uint playerTrophies;
        [SerializeField]
        private string playerIcon;

        public uint Coins { get => coins; set => coins = value; }
        public uint Gems { get => gems; set => gems = value; }
        public ProductUserId PlayerId { get => playerId; set => playerId = value; }
        public string PlayerDisplayName { get => playerDisplayName; set => playerDisplayName = value; }
        public uint PlayerTrophies { get => playerTrophies; set => playerTrophies = value; }
        public string PlayerIcon { get => playerIcon; set => playerIcon = value; }

        // Constructor with default starting values
        public PlayerGameData()
        {
            Coins = 500;
            Gems = 50;
            PlayerId = new ProductUserId();
            PlayerDisplayName = "Player";
            PlayerTrophies = 0;
            PlayerIcon = "Profile_1";
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static PlayerGameData FromJson(string json)
        {
            return JsonUtility.FromJson<PlayerGameData>(json);
        }
    }
}
