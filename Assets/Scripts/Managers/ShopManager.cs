using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static event EventHandler OnShopItemBought;
    public static event EventHandler OnShopItemBoughtFailed;

    // Handle to the shop manager
    public static ShopManager Instance { get; private set; }

    [SerializeField] private int dailyFreebieCount = 3;
    [SerializeField] private int offerItemCount = 0;
    [SerializeField] private int skinItemCount = 3;
    [SerializeField] private int dailyFreebieCountMax = 3;
    [SerializeField] private ShopItemListSO dailyShopItems;
    [SerializeField] private ShopItemListSO inAppPurchaseItems;
    [SerializeField] private ShopItemListSO freebieItems;
    [SerializeField] private ShopItemListSO skinItems;
    [SerializeField] private double offersSectionTime = 168f;
    [SerializeField] private double offersSectionTimer = 0f;
    [SerializeField] private double dailyFreebiesSectionTime = 24f;
    [SerializeField] private double dailyFreebiesSectionTimer = 0f;
    [SerializeField] private double skinsSectionTime = 168f;
    [SerializeField] private double skinsSectionTimer = 0f;

    private double networkDateTime;

    private Dictionary<ShopSection, Dictionary<string, ShopItemSO>> shopItems = new Dictionary<ShopSection, Dictionary<string, ShopItemSO>>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshShopSection(inAppPurchaseItems, inAppPurchaseItems.shopItemslist.Count, ShopSection.Offers);
    }

    private void Update()
    {
        networkDateTime = NTPTime.GetNetworkTime().TimeOfDay.TotalSeconds;
        offersSectionTimer += networkDateTime;
        dailyFreebiesSectionTimer += networkDateTime;
        skinsSectionTimer += networkDateTime;

        if (offersSectionTimer >= offersSectionTime)
        {
            offersSectionTimer = 0f;
            RefreshShopSection(inAppPurchaseItems, offerItemCount, ShopSection.Offers);
        }

        if (dailyFreebiesSectionTimer >= dailyFreebiesSectionTime)
        {
            dailyFreebiesSectionTimer = 0f;
            RefreshShopSection(freebieItems, dailyFreebieCount, ShopSection.DailyFreebies);
        }

        if (skinsSectionTimer >= skinsSectionTime)
        {
            skinsSectionTimer = 0f;
            RefreshShopSection(skinItems, skinItemCount, ShopSection.Skins);
        }
    }

    private void RefreshShopSection(ShopItemListSO Items, int itemCount, ShopSection section)
    {
        Dictionary<string, ShopItemSO> tempItemsDict = new Dictionary<string, ShopItemSO>();

        // Remove old items from the shop items dictionary
        shopItems.Remove(section);

        for (int i = 0; i < itemCount; i++)
        {
            // Add the new items to the shop items dictionary randomly chosen but not the same as the old items
            int randomIndex = UnityEngine.Random.Range(0, Items.shopItemslist.Count);
            // check if the random index skin item is already in the temp dictionary
            if (!tempItemsDict.ContainsKey(Items.shopItemslist[randomIndex].ShopItemID))
            {
                // Add the skin item to the temp dictionary
                tempItemsDict.Add(Items.shopItemslist[randomIndex].ShopItemID, Items.shopItemslist[randomIndex]);
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
        switch (shopItem.ShopItemCostType)
        {
            case ShopItemCostType.Free:
                return AddItemToPlayerInventory(shopItem);
            case ShopItemCostType.Coins:
                if (PlayerInventory.Instance.HasEnoughCoins(shopItem.ShopItemCost))
                {
                    PlayerInventory.Instance.SpendCoins(shopItem.ShopItemCost);
                    return AddItemToPlayerInventory(shopItem);
                }
                else
                {
                    return false;
                }
            case ShopItemCostType.Gems:
                if (PlayerInventory.Instance.HasEnoughGems(shopItem.ShopItemCost))
                {
                    PlayerInventory.Instance.SpendGems(shopItem.ShopItemCost);
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
        switch (shopItem.ShopItemType)
        {
            case ShopItemType.Skin:
                foreach (GameObject itemPrefab in shopItem.ShopItemPrefab)
                {
                    // Convert the skin prefab to a skin object
                    SkinSO skin = itemPrefab.GetComponent<SkinSO>();
                    if (skin != null)
                    {
                        // Add skin to player inventory
                        PlayerInventory.Instance.UnlockSkin(skin);
                        // Show skin unlocked popup
                        OnShopItemBought?.Invoke(this, EventArgs.Empty);
                        return true;
                    }
                }
                break;
            case ShopItemType.Resource:
                foreach (GameObject itemPrefab in shopItem.ShopItemPrefab)
                {
                    // Convert the skin prefab to a skin object
                    ResourceSO resource = itemPrefab.GetComponent<ResourceSO>();
                    if (resource != null)
                    {
                        switch (resource.ResourceType)
                        {
                            case ResourceType.Coin:
                                // Add coins to player inventory
                                PlayerInventory.Instance.AddCoins(resource.ResourceAmount);
                                // Show coins added popup
                                OnShopItemBought?.Invoke(this, EventArgs.Empty);
                                return true;
                            case ResourceType.Gem:
                                // Add gems to player inventory
                                PlayerInventory.Instance.AddGems(resource.ResourceAmount);
                                // Show gems added popup
                                OnShopItemBought?.Invoke(this, EventArgs.Empty);
                                return true;
                        }
                    }
                }
                break;
        }
        OnShopItemBoughtFailed?.Invoke(this, EventArgs.Empty);
        return false;
    }

    public void BuyItem(ShopItemSO shopItem, ShopSection shopSection)
    {
        switch (shopSection)
        {
            case ShopSection.DailyFreebies:
                // Buy daily freebie
                Purchase(shopItem);
                break;
            case ShopSection.Offers:
                // Buy offer
                Purchase(shopItem);
                break;
            case ShopSection.Skins:
                // Buy skin
                Purchase(shopItem);
                break;

            case ShopSection.Resources:
                // Buy resource with In-App Purchase (IAP)

                // Show IAP popup

                // If IAP is successful

                // Add resource to player inventory
                break;
        }
    }

}