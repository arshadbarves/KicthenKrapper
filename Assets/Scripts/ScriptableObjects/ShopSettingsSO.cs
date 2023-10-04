using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "ShopSettings", menuName = "Shop/ShopSettings")]
    public class ShopSettingsSO : ScriptableObject
    {
        [Header("Shop Item Lists")]
        public ShopItemListSO inAppPurchaseItems;
        public ShopItemListSO freebieItems;
        public ShopItemListSO skinItems;

        [Header("Item Counts")]
        public int offerItemCount = 0;
        public int skinItemCount = 3;
        public int dailyFreebieCountMax = 3;

        [Header("Section Timers")]
        public double offersSectionTime = 168f;
        public double dailyFreebiesSectionTime = 24f;
        public double skinsSectionTime = 168f;

        [HideInInspector]
        public double offersSectionTimer = 0f;
        [HideInInspector]
        public double dailyFreebiesSectionTimer = 0f;
        [HideInInspector]
        public double skinsSectionTimer = 0f;

        public int GetItemCountsForSection(ShopSection section)
        {
            switch (section)
            {
                case ShopSection.Offers:
                    return offerItemCount;
                case ShopSection.DailyFreebies:
                    return dailyFreebieCountMax;
                case ShopSection.Skins:
                    return skinItemCount;
                default:
                    return 0;
            }
        }

        public ShopItemListSO GetShopItemListForSection(ShopSection section)
        {
            switch (section)
            {
                case ShopSection.Offers:
                    return inAppPurchaseItems;
                case ShopSection.DailyFreebies:
                    return freebieItems;
                case ShopSection.Skins:
                    return skinItems;
                default:
                    return null;
            }
        }
    }
}
