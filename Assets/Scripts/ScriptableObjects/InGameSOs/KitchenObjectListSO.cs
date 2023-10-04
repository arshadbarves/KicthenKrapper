using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "KitchenObjectListSO", menuName = "ScriptableObjects/KitchenObjectListSO")]
    public class KitchenObjectListSO : ScriptableObject
    {
        public List<KitchenObjectSO> kitchenObjectSOList;
    }
}