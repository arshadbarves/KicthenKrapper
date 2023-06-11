
using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    public static new void ResetStaticData()
    {
        OnAnyCut = null;
    }
    public event EventHandler<IHasProgress.ProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                // Player has a kitchen object, so set it as this counter's kitchen object
                if (HasCuttingRecipe(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    InteractServerRpc();

                    StepComplete();
                }
            }
            else
            {
                // Player doesn't have a kitchen object, so spawn one
            }
        }
        else
        {
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
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);

                StepComplete();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }

    [ClientRpc]
    private void InteractClientRpc()
    {
        cuttingProgress = 0;
        OnProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs { progressNormalized = 0f });
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()))
        {
            // Cut the kitchen object if it has a cutting recipe
            InteractAlternateServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractAlternateServerRpc()
    {
        if (HasKitchenObject() && HasCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAlternateClientRpc();
        }
    }

    [ClientRpc]
    public void InteractAlternateClientRpc()
    {
        cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs
        {
            progressNormalized = (float)
        cuttingProgress / cuttingRecipeSO.cuttingProgressRequired
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        if (HasKitchenObject() && HasCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()))
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(GetKitchenObject().GetKitchenObjectSO());

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressRequired)
            {
                // Cutting is complete, so spawn the output kitchen object
                StepComplete();

                KitchenObjectSO kitchenObjectSO = GetOutputKitchenObjectSO(GetKitchenObject().GetKitchenObjectSO());

                KitchenObject.DestroyKitchenObject(GetKitchenObject());

                if (kitchenObjectSO != null)
                {
                    KitchenObject.CreateKitchenObject(kitchenObjectSO, this);
                }
            }
        }
    }

    private bool HasCuttingRecipe(KitchenObjectSO inputKitchenObjectSO)
    {
        return GetCuttingRecipeSO(inputKitchenObjectSO) != null;
    }

    private KitchenObjectSO GetOutputKitchenObjectSO(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.outputKitchenObjectSO;
        }

        return null;
    }

    private CuttingRecipeSO GetCuttingRecipeSO(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.inputKitchenObjectSO == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
