using UnityEngine;

namespace KitchenKrapper
{
    public class ClearCounter : BaseStation
    {
        [SerializeField] private KitchenObjectSO kitchenObjectSO;

        public override void Interact(PlayerController player)
        {
            if (!HasKitchenObject())
            {
                HandleCounterEmpty(player);
            }
            else
            {
                HandleCounterOccupied(player);
            }
        }

        private void HandleCounterEmpty(PlayerController player)
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                StepComplete();
            }
            // No need for an else clause here since we're not doing anything if both are empty.
        }

        private void HandleCounterOccupied(PlayerController player)
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlateKitchenObject(out PlateKitchenObject playerPlate))
                {
                    TryAddIngredientToPlate(player, playerPlate);
                }
                else if (GetKitchenObject().TryGetPlateKitchenObject(out PlateKitchenObject counterPlate))
                {
                    TryAddIngredientToPlate(this, counterPlate);
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                StepComplete();
            }
        }

        private void TryAddIngredientToPlate(IKitchenObjectParent sourceParent, PlateKitchenObject plate)
        {
            if (plate.TryAddIngredient(sourceParent.GetKitchenObject().GetKitchenObjectSO()))
            {
                KitchenObject.DestroyKitchenObject(sourceParent.GetKitchenObject());
                StepComplete();
            }
        }
    }
}