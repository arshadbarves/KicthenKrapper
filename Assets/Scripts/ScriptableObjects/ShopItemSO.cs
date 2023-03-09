using System.Collections.Generic;
using UnityEngine;

public class ShopItemSO : ScriptableObject
{
    [SerializeField] private string shopItemID = UniqueIDGenerator.GetUniqueID();
    [SerializeField] private Sprite shopItemSprite;
    [SerializeField] private string shopItemName;
    [SerializeField] private string shopItemDescription;
    [SerializeField] private ShopItemCostType shopItemCostType;
    [SerializeField] private int shopItemCost;
    [SerializeField] private ShopItemType shopItemType;
    [SerializeField] private List<GameObject> shopItemPrefab;

    // Getters
    public string ShopItemID => shopItemID;
    public Sprite ShopItemSprite => shopItemSprite;
    public string ShopItemName => shopItemName;
    public string ShopItemDescription => shopItemDescription;
    public int ShopItemCost => shopItemCost;
    public ShopItemCostType ShopItemCostType => shopItemCostType;
    public ShopItemType ShopItemType => shopItemType;
    public List<GameObject> ShopItemPrefab => shopItemPrefab;
}
