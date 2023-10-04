using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "New Burning Recipe", menuName = "ScriptableObjects/Burning Recipe")]
    public class BurningRecipeSO : ScriptableObject
    {
        public KitchenObjectSO inputFoodObjectSO;
        public KitchenObjectSO outputFoodObjectSO;
        public float burningTime;
    }
}
