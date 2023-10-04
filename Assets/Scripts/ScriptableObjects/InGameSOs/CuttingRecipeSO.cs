using UnityEngine;

namespace KitchenKrapper
{
    [CreateAssetMenu(fileName = "New Cutting Recipe", menuName = "ScriptableObjects/Cutting Recipe")]
    public class CuttingRecipeSO : ScriptableObject
    {
        public KitchenObjectSO inputKitchenObjectSO;
        public KitchenObjectSO outputKitchenObjectSO;
        public int cuttingProgressRequired;
    }
}