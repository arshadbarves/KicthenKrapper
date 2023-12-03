using System;
using System.Collections.Generic;
using KitchenKrapper;
using UnityEngine;
using Utils.Enums;

namespace Managers
{
    public class ShopManager : Singleton<ShopManager>
    {
        public static event EventHandler OnShopItemBought;
        public static event EventHandler OnShopItemBoughtFailed;

        [Header("Shop Settings")]
        [SerializeField] private ShopSettingsSO shopSettings;

        private double networkDateTime;

        private Dictionary<ShopSection, Dictionary<string, ShopItemSO>> shopItems = new Dictionary<ShopSection, Dictionary<string, ShopItemSO>>();

        private void Start()
        {
            RefreshShopSection(ShopSection.Offers);
        }

        private void Update()
        {
            networkDateTime = NTPTime.GetNetworkTime().TimeOfDay.TotalSeconds;

            UpdateShopSectionTimer(ref shopSettings.offersSectionTimer, shopSettings.offersSectionTime, ShopSection.Offers);
            UpdateShopSectionTimer(ref shopSettings.dailyFreebiesSectionTimer, shopSettings.dailyFreebiesSectionTime, ShopSection.DailyFreebies);
            UpdateShopSectionTimer(ref shopSettings.skinsSectionTimer, shopSettings.skinsSectionTime, ShopSection.Skins);
        }

        private void UpdateShopSectionTimer(ref double sectionTimer, double sectionTime, ShopSection section)
        {
            sectionTimer += networkDateTime;

            if (sectionTimer >= sectionTime)
            {
                sectionTimer = 0f;
                RefreshShopSection(section);
            }
        }

        private void RefreshShopSection(ShopSection section)
        {
            Dictionary<string, ShopItemSO> tempItemsDict = new Dictionary<string, ShopItemSO>();

            // Remove old items from the shop items dictionary
            shopItems.Remove(section);

            for (int i = 0; i < shopSettings.GetItemCountsForSection(section); i++)
            {
                // Add the new items to the shop items dictionary randomly chosen but not the same as the old items
                int randomIndex = UnityEngine.Random.Range(0, shopSettings.GetShopItemListForSection(section).shopItemslist.Count);

                if (!tempItemsDict.ContainsKey(shopSettings.GetShopItemListForSection(section).shopItemslist[randomIndex].itemID))
                {
                    tempItemsDict.Add(shopSettings.GetShopItemListForSection(section).shopItemslist[randomIndex].itemID, shopSettings.GetShopItemListForSection(section).shopItemslist[randomIndex]);
                }
            }

            // Add the new items to the shop items dictionary
            shopItems.Add(section, tempItemsDict);
        }

        public Dictionary<string, ShopItemSO> GetShopItems(ShopSection shopSection)
        {
            return shopItems[shopSection];
        }

        private bool Purchase(ShopItemSO shopItem)
        {
            switch (shopItem.CostInCurrencyType)
            {
                case CurrencyType.Free:
                    return AddItemToPlayerInventory(shopItem);
                case CurrencyType.Coin:
                    if (PlayerInventory.Instance.HasEnoughCoins((int)shopItem.cost))
                    {
                        PlayerInventory.Instance.SpendCoins((int)shopItem.cost);
                        return AddItemToPlayerInventory(shopItem);
                    }
                    else
                    {
                        return false;
                    }
                case CurrencyType.Gems:
                    if (PlayerInventory.Instance.HasEnoughGems((int)shopItem.cost))
                    {
                        PlayerInventory.Instance.SpendGems((int)shopItem.cost);
                        return AddItemToPlayerInventory(shopItem);
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        private bool AddItemToPlayerInventory(ShopItemSO shopItem)
        {
            switch (shopItem.contentType)
            {
                case KitchenKrapper.ShopItemType.Skin:
                    // Handle skin unlocking
                    break;
                case KitchenKrapper.ShopItemType.Coin:
                    PlayerInventory.Instance.AddCoins((int)shopItem.cost);
                    break;
                case KitchenKrapper.ShopItemType.Gems:
                    PlayerInventory.Instance.AddGems((int)shopItem.cost);
                    break;
            }

            OnShopItemBought?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public void BuyItem(ShopItemSO shopItem, ShopSection shopSection)
        {
            Purchase(shopItem);
        }
    }
}
