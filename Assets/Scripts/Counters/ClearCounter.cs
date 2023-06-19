using UnityEngine;

public class ClearCounter : BaseStation
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {

        if (!HasKitchenObject())
        {
            // Counter doesn't have a kitchen object
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                StepComplete();
            }
            else
            {
                // Player doesn't have a kitchen object
            }
        }
        else
        {
            // Counter already has a kitchen object
            if (player.HasKitchenObject())
            {
                // Player already has a kitchen object
                if (player.GetKitchenObject().TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                {
                    // Player has a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        // Ingredient added to plate
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        StepComplete();
                    }
                }
                else
                {
                    // Player doesn't have a plate
                    if (GetKitchenObject().TryGetPlateKitchenObject(out plateKitchenObject))
                    {
                        // Counter has a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            // Ingredient added to plate
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                            StepComplete();
                        }
                    }
                }
            }
            else
            {
                // Player doesn't have a kitchen object
                GetKitchenObject().SetKitchenObjectParent(player);

                StepComplete();
            }
        }
    }
}
