using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KitchenKrapper
{
    [Serializable]
    public struct CurrencyIcon
    {
        public Sprite icon;
        public CurrencyType currencyType;
    }

    [Serializable]
    public struct ShopItemTypeIcon
    {
        public Sprite icon;
        public ShopItemType shopItemType;
    }

    [Serializable]
    public struct CharacterClassIcon
    {
        public Sprite icon;
        public CharacterClass characterClass;
    }

    // returns an icon matching a ShopItem,CurrencyIcon, CharacterClass, Rarity, or AttackType
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Icons", menuName = "KitchenKrapper/Icons", order = 10)]
    public class GameIconsSO : ScriptableObject
    {
        public List<CurrencyIcon> currencyIcons;
        public List<ShopItemTypeIcon> shopItemTypeIcons;
        public List<CharacterClassIcon> characterClassIcons;

        public Sprite GetCurrencyIcon(CurrencyType currencyType)
        {
            if (currencyIcons == null || currencyIcons.Count == 0)
                return null;

            CurrencyIcon match = currencyIcons.Find(x => x.currencyType == currencyType);
            return match.icon;
        }

        public Sprite GetShopTypeIcon(ShopItemType shopItemType)
        {
            if (shopItemTypeIcons == null || shopItemTypeIcons.Count == 0)
                return null;

            ShopItemTypeIcon match = shopItemTypeIcons.Find(x => x.shopItemType == shopItemType);
            return match.icon;
        }

        public Sprite GetCharacterClassIcon(CharacterClass charClass)
        {
            if (characterClassIcons == null || characterClassIcons.Count == 0)
                return null;

            CharacterClassIcon match = characterClassIcons.Find(x => x.characterClass == charClass);
            return match.icon;
        }
    }
}
