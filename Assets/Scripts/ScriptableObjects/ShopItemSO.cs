using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    // what player is buying
    [System.Serializable]
    public enum ShopItemType
    {
        // soft currency (in-game)
        Coin,

        // hard currency (buy with real money)
        Gems,

        // used in gameplay
        Skin
    }

    // type of currency used to purchase
    [System.Serializable]
    public enum CurrencyType
    {
        Free,
        Coin,
        Gems,
        // use real money to buy gems
        USD
    }

    [CreateAssetMenu(fileName = "Assets/Resources/GameData/ShopItems/ShopItemGameData", menuName = "KitchenKrapper/ShopItem", order = 4)]
    public class ShopItemSO : ScriptableObject
    {
        public string itemID;
        public string itemName;

        public Sprite sprite;

        // FREE if equal to 0; cost amount in CostInCurrencyType below
        public float cost;

        // UI shows tag if value larger than 0 (percentage off)
        public uint discount;

        // if not empty, UI shows a banner with this text
        public string promoBannerText;

        // how many potions/coins this item gives the player upon purchase
        public uint contentValue;
        public ShopItemType contentType;

        // SC (gold) costs HC (gems); HC (gems) costs real USD; HealthPotion costs SC (gold); LevelUpPotion costs HC (gems)
        public CurrencyType CostInCurrencyType
        {
            get
            {
                switch (contentType)
                {
                    case (ShopItemType.Coin):
                        return CurrencyType.Gems;

                    case (ShopItemType.Gems):
                        return CurrencyType.USD;

                    case (ShopItemType.Skin):
                        return CurrencyType.Gems;

                    default:
                        return CurrencyType.Gems;
                }
            }
        }
    }
}