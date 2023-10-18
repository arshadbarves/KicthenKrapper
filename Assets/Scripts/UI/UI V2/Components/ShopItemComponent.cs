using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace KitchenKrapper
{
    // represents one item in the shop
    public class ShopItemComponent
    {

        // notify the ShopScreenController to buy product (passes the ShopItem data + UI element screen position)
        public static event Action<ShopItemSO, Vector2> ShopItemClicked;

        // string IDs
        const string PARENT_CONTAINER_NAME = "shop-item__parent-container";
        const string DESCRIPTION_LABEL_NAME = "shop-item__description";
        const string PRODUCT_IMAGE_NAME = "shop-item__product-image";

        const string BANNER_NAME = "shop-item__banner";
        const string BANNER_LABEL_NAME = "shop-item__banner-label";

        const string DISCOUNT_BADGE_NAME = "shop-item__discount-badge";
        const string DISCOUNT_LABEL_NAME = "shop-item__badge-text";
        const string DISCOUNT_SLASH_NAME = "shop-item__discount-slash";

        const string CONTENT_CURRENCY_LABEL_NAME = "shop-item__content-currency";
        const string CONTENT_VALUE_LABEL_NAME = "shop-item__content-value";

        const string COST_ICON_NAME = "shop-item__cost-icon";
        const string COST_ICON_GROUP_NAME = "shop-item__cost-icon-group";
        const string COST_PRICE_LABEL_NAME = "shop-item__cost-price";

        const string DISCOUNT_ICON_NAME = "shop-item__discount-icon";
        const string DISCOUNT_ICON_GROUP = "shop-item__discount-icon-group";
        const string DISCOUNT_PRICE_LABEL_NAME = "shop-item__discount-price";
        const string DISCOUNT_GROUP_NAME = "shop-item__discount-group";

        const string BUY_BUTTON_NAME = "shop-item__buy-button";

        const string SIZE_NORMAL_CLASS_NAME = "shop-item__size--normal";
        const string SIZE_WIDE_CLASS_NAME = "shop-item__size--wide";

        // ScriptableObject pairing icons with currency/shop item types
        GameIconsSO m_GameIconsData;
        ShopItemSO m_ShopItemData;

        // visual elements
        Label m_Description;
        VisualElement m_ProductImage;
        VisualElement m_Banner;
        Label m_BannerLabel;
        VisualElement m_ContentCurrency;
        Label m_ContentValue;
        VisualElement m_CostIcon;
        Label m_Cost;
        VisualElement m_DiscountBadge;
        Label m_DiscountLabel;
        VisualElement m_DiscountSlash;
        VisualElement m_DiscountIcon;
        VisualElement m_DiscountGroup;
        VisualElement m_SizeContainer;
        Label m_DiscountCost;
        Button m_BuyButton;
        VisualElement m_CostIconGroup;
        VisualElement m_DiscountIconGroup;

        public ShopItemComponent(GameIconsSO gameIconsData, ShopItemSO shopItemData)
        {
            m_GameIconsData = gameIconsData;
            m_ShopItemData = shopItemData;
        }

        public void SetVisualElements(TemplateContainer shopItemElement)
        {
            // query the parts of the ShopItemElement
            m_SizeContainer = shopItemElement.Q(PARENT_CONTAINER_NAME);
            m_Description = shopItemElement.Q<Label>(DESCRIPTION_LABEL_NAME);
            m_ProductImage = shopItemElement.Q(PRODUCT_IMAGE_NAME);
            m_Banner = shopItemElement.Q(BANNER_NAME);
            m_BannerLabel = shopItemElement.Q<Label>(BANNER_LABEL_NAME);
            m_DiscountBadge = shopItemElement.Q(DISCOUNT_BADGE_NAME);
            m_DiscountLabel = shopItemElement.Q<Label>(DISCOUNT_LABEL_NAME);
            m_DiscountSlash = shopItemElement.Q(DISCOUNT_SLASH_NAME);
            m_ContentCurrency = shopItemElement.Q(CONTENT_CURRENCY_LABEL_NAME);
            m_ContentValue = shopItemElement.Q<Label>(CONTENT_VALUE_LABEL_NAME);
            m_CostIcon = shopItemElement.Q(COST_ICON_NAME);
            m_Cost = shopItemElement.Q<Label>(COST_PRICE_LABEL_NAME);
            m_DiscountIcon = shopItemElement.Q(DISCOUNT_ICON_NAME);
            m_DiscountGroup = shopItemElement.Q(DISCOUNT_GROUP_NAME);
            m_DiscountCost = shopItemElement.Q<Label>(DISCOUNT_PRICE_LABEL_NAME);
            m_BuyButton = shopItemElement.Q<Button>(BUY_BUTTON_NAME);

            m_CostIconGroup = shopItemElement.Q(COST_ICON_GROUP_NAME);
            m_DiscountIconGroup = shopItemElement.Q(DISCOUNT_ICON_GROUP);
        }

        // show the ScriptaboleObject data
        public void SetGameData(TemplateContainer shopItemElement)
        {
            if (m_GameIconsData == null)
            {
                Debug.LogWarning("ShopItemController SetGameData: missing GameIcons ScriptableObject data");
                return;
            }

            if (shopItemElement == null)
                return;

            // basic description and image
            m_Description.text = m_ShopItemData.itemName;
            m_ProductImage.style.backgroundImage = new StyleBackground(m_ShopItemData.sprite);

            // set up the promo banner
            m_Banner.style.display = (HasBanner(m_ShopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            m_BannerLabel.style.display = (HasBanner(m_ShopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            m_BannerLabel.text = m_ShopItemData.promoBannerText;

            // content value
            m_ContentCurrency.style.backgroundImage = new StyleBackground(m_GameIconsData.GetShopTypeIcon(m_ShopItemData.contentType));
            m_ContentValue.text = " " + m_ShopItemData.contentValue.ToString();

            FormatBuyButton();

            // use the oversize style if discounted
            if (IsDiscounted(m_ShopItemData))
            {
                m_SizeContainer.AddToClassList(SIZE_WIDE_CLASS_NAME);
                m_SizeContainer.RemoveFromClassList(SIZE_NORMAL_CLASS_NAME);
            }
            else
            {
                m_SizeContainer.AddToClassList(SIZE_NORMAL_CLASS_NAME);
                m_SizeContainer.RemoveFromClassList(SIZE_WIDE_CLASS_NAME);
            }
        }

        // format the cost and cost currency
        void FormatBuyButton()
        {
            string currencyPrefix = (m_ShopItemData.CostInCurrencyType == CurrencyType.USD) ? "$" : string.Empty;
            string decimalPlaces = (m_ShopItemData.CostInCurrencyType == CurrencyType.USD) ? "0.00" : "0";

            if (m_ShopItemData.cost > 0.00001f)
            {
                m_Cost.text = currencyPrefix + m_ShopItemData.cost.ToString(decimalPlaces);
                Sprite currencySprite = m_GameIconsData.GetCurrencyIcon(m_ShopItemData.CostInCurrencyType);

                m_CostIcon.style.backgroundImage = new StyleBackground(currencySprite);
                m_DiscountIcon.style.backgroundImage = new StyleBackground(currencySprite);

                m_CostIconGroup.style.display = (m_ShopItemData.CostInCurrencyType == CurrencyType.USD) ? DisplayStyle.None : DisplayStyle.Flex;
                m_DiscountIconGroup.style.display = (m_ShopItemData.CostInCurrencyType == CurrencyType.USD) ? DisplayStyle.None : DisplayStyle.Flex;

            }
            // if the cost is 0, mark the ShopItem as free and hide the cost currency
            else
            {
                m_CostIconGroup.style.display = DisplayStyle.None;
                m_DiscountIconGroup.style.display = DisplayStyle.None;
                m_Cost.text = "Free";
            }

            // disable/enabled, depending whether the item is discounted
            m_DiscountBadge.style.display = (IsDiscounted(m_ShopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            m_DiscountLabel.text = m_ShopItemData.discount + "%";
            m_DiscountSlash.style.display = (IsDiscounted(m_ShopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            m_DiscountGroup.style.display = (IsDiscounted(m_ShopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            m_DiscountCost.text = currencyPrefix + (((100 - m_ShopItemData.discount) / 100f) * m_ShopItemData.cost).ToString(decimalPlaces);
        }

        bool IsDiscounted(ShopItemSO shopItem)
        {
            return (shopItem.discount > 0);
        }

        bool HasBanner(ShopItemSO shopItem)
        {
            return !string.IsNullOrEmpty(shopItem.promoBannerText);
        }

        public void RegisterCallbacks()
        {
            if (m_BuyButton == null)
                return;

            // store the cost/contents data in each button for later use
            m_BuyButton.userData = m_ShopItemData;
            m_BuyButton.RegisterCallback<ClickEvent>(BuyAction);
            m_BuyButton.RegisterCallback<PointerMoveEvent>(MovePointerEventHanlder);
        }

        void MovePointerEventHanlder(PointerMoveEvent evt)
        {
            // prevents accidental left-right movement of the mouse from dragging the parent Scrollview
            evt.StopImmediatePropagation();
        }

        void BuyAction(ClickEvent evt)
        {
            VisualElement clickedElement = evt.currentTarget as VisualElement;
            ShopItemSO shopItemData = clickedElement.userData as ShopItemSO;

            // starts a chain of events:

            //      ShopItemComponent (click the button) -->
            //      ShopController (buy an item) -->
            //      GameDataManager (verify funds)-->
            //      MagnetFXController (play effect on UI)

            // notify the ShopController (passes ShopItem data + UI Toolkit screen position)
            Vector2 screenPos = clickedElement.worldBound.center;

            ShopItemClicked?.Invoke(shopItemData, screenPos);

            AudioManager.Instance.PlayDefaultButtonSound();
        }
    }
}

