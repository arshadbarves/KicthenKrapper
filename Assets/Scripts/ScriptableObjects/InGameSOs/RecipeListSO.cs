using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "New Recipe List", menuName = "ScriptableObjects/Recipe List")]
    public class RecipeListSO : ScriptableObject
    {
        public List<RecipeSO> recipeSOList;
    }
}