using System;
using Managers;
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
        private const string ParentContainerName = "shop-item__parent-container";
        private const string DescriptionLabelName = "shop-item__description";
        private const string ProductImageName = "shop-item__product-image";

        private const string BannerName = "shop-item__banner";
        private const string BannerLabelName = "shop-item__banner-label";

        private const string DiscountBadgeName = "shop-item__discount-badge";
        private const string DiscountLabelName = "shop-item__badge-text";
        private const string DiscountSlashName = "shop-item__discount-slash";

        private const string ContentCurrencyLabelName = "shop-item__content-currency";
        private const string ContentValueLabelName = "shop-item__content-value";

        private const string CostIconName = "shop-item__cost-icon";
        private const string CostIconGroupName = "shop-item__cost-icon-group";
        private const string CostPriceLabelName = "shop-item__cost-price";

        private const string DiscountIconName = "shop-item__discount-icon";
        private const string DiscountIconGroup = "shop-item__discount-icon-group";
        private const string DiscountPriceLabelName = "shop-item__discount-price";
        private const string DiscountGroupName = "shop-item__discount-group";

        private const string BuyButtonName = "shop-item__buy-button";

        private const string SizeNormalClassName = "shop-item__size--normal";
        private const string SizeWideClassName = "shop-item__size--wide";

        // ScriptableObject pairing icons with currency/shop item types
        private readonly GameIconsSO _gameIconsData;
        private readonly ShopItemSO _shopItemData;

        // visual elements
        private Label _mDescription;
        private VisualElement _mProductImage;
        private VisualElement _mBanner;
        private Label _mBannerLabel;
        private VisualElement _mContentCurrency;
        private Label _mContentValue;
        private VisualElement _mCostIcon;
        private Label _mCost;
        private VisualElement _mDiscountBadge;
        private Label _mDiscountLabel;
        private VisualElement _mDiscountSlash;
        private VisualElement _mDiscountIcon;
        private VisualElement _mDiscountGroup;
        private VisualElement _mSizeContainer;
        private Label _mDiscountCost;
        private Button _mBuyButton;
        private VisualElement _mCostIconGroup;
        private VisualElement _mDiscountIconGroup;

        public ShopItemComponent(GameIconsSO gameIconsData, ShopItemSO shopItemData)
        {
            _gameIconsData = gameIconsData;
            _shopItemData = shopItemData;
        }

        public void SetVisualElements(TemplateContainer shopItemElement)
        {
            // query the parts of the ShopItemElement
            _mSizeContainer = shopItemElement.Q(ParentContainerName);
            _mDescription = shopItemElement.Q<Label>(DescriptionLabelName);
            _mProductImage = shopItemElement.Q(ProductImageName);
            _mBanner = shopItemElement.Q(BannerName);
            _mBannerLabel = shopItemElement.Q<Label>(BannerLabelName);
            _mDiscountBadge = shopItemElement.Q(DiscountBadgeName);
            _mDiscountLabel = shopItemElement.Q<Label>(DiscountLabelName);
            _mDiscountSlash = shopItemElement.Q(DiscountSlashName);
            _mContentCurrency = shopItemElement.Q(ContentCurrencyLabelName);
            _mContentValue = shopItemElement.Q<Label>(ContentValueLabelName);
            _mCostIcon = shopItemElement.Q(CostIconName);
            _mCost = shopItemElement.Q<Label>(CostPriceLabelName);
            _mDiscountIcon = shopItemElement.Q(DiscountIconName);
            _mDiscountGroup = shopItemElement.Q(DiscountGroupName);
            _mDiscountCost = shopItemElement.Q<Label>(DiscountPriceLabelName);
            _mBuyButton = shopItemElement.Q<Button>(BuyButtonName);

            _mCostIconGroup = shopItemElement.Q(CostIconGroupName);
            _mDiscountIconGroup = shopItemElement.Q(DiscountIconGroup);
        }

        // show the ScriptableObject data
        public void SetGameData(TemplateContainer shopItemElement)
        {
            if (_gameIconsData == null)
            {
                Debug.LogWarning("ShopItemController SetGameData: missing GameIcons ScriptableObject data");
                return;
            }

            if (shopItemElement == null)
                return;

            // basic description and image
            _mDescription.text = _shopItemData.itemName;
            _mProductImage.style.backgroundImage = new StyleBackground(_shopItemData.sprite);

            // set up the promo banner
            _mBanner.style.display = (HasBanner(_shopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            _mBannerLabel.style.display = (HasBanner(_shopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            _mBannerLabel.text = _shopItemData.promoBannerText;

            // content value
            _mContentCurrency.style.backgroundImage = new StyleBackground(_gameIconsData.GetShopTypeIcon(_shopItemData.contentType));
            _mContentValue.text = " " + _shopItemData.contentValue.ToString();

            FormatBuyButton();

            // use the oversize style if discounted
            if (IsDiscounted(_shopItemData))
            {
                _mSizeContainer.AddToClassList(SizeWideClassName);
                _mSizeContainer.RemoveFromClassList(SizeNormalClassName);
            }
            else
            {
                _mSizeContainer.AddToClassList(SizeNormalClassName);
                _mSizeContainer.RemoveFromClassList(SizeWideClassName);
            }
        }

        // format the cost and cost currency
        private void FormatBuyButton()
        {
            var currencyPrefix = (_shopItemData.CostInCurrencyType == CurrencyType.USD) ? "$" : string.Empty;
            var decimalPlaces = (_shopItemData.CostInCurrencyType == CurrencyType.USD) ? "0.00" : "0";

            if (_shopItemData.cost > 0.00001f)
            {
                _mCost.text = currencyPrefix + _shopItemData.cost.ToString(decimalPlaces);
                var currencySprite = _gameIconsData.GetCurrencyIcon(_shopItemData.CostInCurrencyType);

                _mCostIcon.style.backgroundImage = new StyleBackground(currencySprite);
                _mDiscountIcon.style.backgroundImage = new StyleBackground(currencySprite);

                _mCostIconGroup.style.display = (_shopItemData.CostInCurrencyType == CurrencyType.USD) ? DisplayStyle.None : DisplayStyle.Flex;
                _mDiscountIconGroup.style.display = (_shopItemData.CostInCurrencyType == CurrencyType.USD) ? DisplayStyle.None : DisplayStyle.Flex;

            }
            // if the cost is 0, mark the ShopItem as free and hide the cost currency
            else
            {
                _mCostIconGroup.style.display = DisplayStyle.None;
                _mDiscountIconGroup.style.display = DisplayStyle.None;
                _mCost.text = "Free";
            }

            // disable/enabled, depending whether the item is discounted
            _mDiscountBadge.style.display = (IsDiscounted(_shopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            _mDiscountLabel.text = _shopItemData.discount + "%";
            _mDiscountSlash.style.display = (IsDiscounted(_shopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            _mDiscountGroup.style.display = (IsDiscounted(_shopItemData)) ? DisplayStyle.Flex : DisplayStyle.None;
            _mDiscountCost.text = currencyPrefix + (((100 - _shopItemData.discount) / 100f) * _shopItemData.cost).ToString(decimalPlaces);
        }

        private bool IsDiscounted(ShopItemSO shopItem)
        {
            return (shopItem.discount > 0);
        }

        private bool HasBanner(ShopItemSO shopItem)
        {
            return !string.IsNullOrEmpty(shopItem.promoBannerText);
        }

        public void RegisterCallbacks()
        {
            if (_mBuyButton == null)
                return;

            // store the cost/contents data in each button for later use
            _mBuyButton.userData = _shopItemData;
            _mBuyButton.RegisterCallback<ClickEvent>(BuyAction);
            _mBuyButton.RegisterCallback<PointerMoveEvent>(MovePointerEventHandler);
        }

        private void MovePointerEventHandler(PointerMoveEvent evt)
        {
            // prevents accidental left-right movement of the mouse from dragging the parent Scrollview
            evt.StopImmediatePropagation();
        }

        private void BuyAction(ClickEvent evt)
        {
            var clickedElement = evt.currentTarget as VisualElement;
            var shopItemData = clickedElement?.userData as ShopItemSO;

            // starts a chain of events:

            //      ShopItemComponent (click the button) -->
            //      ShopController (buy an item) -->
            //      GameDataManager (verify funds)-->
            //      MagnetFXController (play effect on UI)

            // notify the ShopController (passes ShopItem data + UI Toolkit screen position)
            var screenPos = clickedElement!.worldBound.center;

            ShopItemClicked?.Invoke(shopItemData, screenPos);

            AudioManager.Instance.PlayDefaultButtonSound();
        }
    }
}

