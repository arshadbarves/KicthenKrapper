using System;
using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class CuttingCounter : BaseStation, IHasProgress
    {
        public static event EventHandler OnAnyCut;

        public event EventHandler<IHasProgress.ProgressChangedEventArgs> OnProgressChanged;
        public event EventHandler OnCut;

        [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

        private int cuttingProgress;

        public static void ResetStaticData()
        {
            OnAnyCut = null;
        }

        public override void Interact(PlayerController player)
        {
            if (!HasKitchenObject())
            {
                HandleKitchenObjectInteraction(player);
            }
            else
            {
                HandleExistingKitchenObjectInteraction(player);
            }
        }

        private void HandleKitchenObjectInteraction(PlayerController player)
        {
            if (player.HasKitchenObject())
            {
                HandleKitchenObjectPickup(player);
            }
            else
            {
                // Player doesn't have a kitchen object, so spawn one
            }
        }

        private void HandleKitchenObjectPickup(PlayerController player)
        {
            if (HasCuttingRecipe(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);
                InteractServerRpc();
                StepComplete();
            }
        }

        private void HandleExistingKitchenObjectInteraction(PlayerController player)
        {
            if (player.HasKitchenObject())
            {
                HandleExistingPlayerKitchenObject(player);
            }
            else
            {
                HandleNoPlayerKitchenObject(player);
            }
        }

        private void HandleExistingPlayerKitchenObject(PlayerController player)
        {
            if (player.GetKitchenObject().TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
            {
                HandlePlateKitchenObject(player, plateKitchenObject);
            }
        }

        private void HandlePlateKitchenObject(PlayerController player, PlateKitchenObject plateKitchenObject)
        {
            if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
            {
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                StepComplete();
            }
        }

        private void HandleNoPlayerKitchenObject(PlayerController player)
        {
            GetKitchenObject().SetKitchenObjectParent(player);
            StepComplete();
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

        public override void InteractAlternate(PlayerController player)
        {
            if (HasKitchenObject() && HasCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()))
            {
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
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressRequired
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
                    HandleCuttingCompletion(cuttingRecipeSO);
                }
            }
        }

        private void HandleCuttingCompletion(CuttingRecipeSO cuttingRecipeSO)
        {
            StepComplete();

            KitchenObjectSO kitchenObjectSO = GetOutputKitchenObjectSO(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            if (kitchenObjectSO != null)
            {
                KitchenObject.CreateKitchenObject(kitchenObjectSO, this);
            }
        }

        private bool HasCuttingRecipe(KitchenObjectSO inputKitchenObjectSO)
        {
            return GetCuttingRecipeSO(inputKitchenObjectSO) != null;
        }

        private KitchenObjectSO GetOutputKitchenObjectSO(KitchenObjectSO inputKitchenObjectSO)
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(inputKitchenObjectSO);
            return cuttingRecipeSO?.outputKitchenObjectSO;
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
}