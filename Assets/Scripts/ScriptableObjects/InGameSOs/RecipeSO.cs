using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "ScriptableObjects/Recipe")]
    public class RecipeSO : ScriptableObject
    {
        public List<KitchenObjectSO> kitchenObjectSOList;
        public string recipeName;
    }
}