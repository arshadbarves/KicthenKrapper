using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "New Resource", menuName = "ScriptableObjects/Resource")]
    public class ResourceSO : ScriptableObject
    {
        [SerializeField] private string resourceID = UniqueIDGenerator.GetUniqueID();
        [SerializeField] private Sprite resourceSprite;
        [SerializeField] private string resourceName;
        [SerializeField] private string resourceDescription;
        [SerializeField] private int resourceCost;
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private int resourceAmount;

        // Getters
        public string ResourceID => resourceID;
        public Sprite ResourceSprite => resourceSprite;
        public string ResourceName => resourceName;
        public string ResourceDescription => resourceDescription;
        public int ResourceCost => resourceCost;
        public ResourceType ResourceType => resourceType;
        public int ResourceAmount => resourceAmount;
    }
}