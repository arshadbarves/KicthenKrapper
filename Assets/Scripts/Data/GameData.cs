using Epic.OnlineServices;
using UnityEngine;

namespace KitchenKrapper
{
    [System.Serializable]
    public class GameData
    {
        private uint coins;
        private uint gems;
        private ProductUserId playerId;
        private string playerDisplayName;
        private uint playerTrophies;
        private string playerIcon;

        public uint Coins { get => coins; set => coins = value; }
        public uint Gems { get => gems; set => gems = value; }
        public ProductUserId PlayerId { get => playerId; set => playerId = value; }
        public string PlayerDisplayName { get => playerDisplayName; set => playerDisplayName = value; }
        public uint PlayerTrophies { get => playerTrophies; set => playerTrophies = value; }
        public string PlayerIcon { get => playerIcon; set => playerIcon = value; }


        // constructor, starting values
        public GameData()
        {
            this.coins = 500;
            this.gems = 50;
            this.playerId = new ProductUserId();
            this.playerDisplayName = "Player";
            this.playerTrophies = 0;
            this.playerIcon = "Default";
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public void LoadJson(string jsonFilepath)
        {
            JsonUtility.FromJsonOverwrite(jsonFilepath, this);
        }
    }
}