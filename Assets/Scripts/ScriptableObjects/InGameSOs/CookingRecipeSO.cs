using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "New Cooking Recipe", menuName = "ScriptableObjects/Cooking Recipe")]
    public class CookingRecipeSO : ScriptableObject
    {
        public KitchenObjectSO inputFoodObjectSO;
        public KitchenObjectSO outputFoodObjectSO;
        public float cookingTime;
    }
}