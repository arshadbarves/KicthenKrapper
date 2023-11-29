using System;
using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class StoveCounter : BaseStation, IHasProgress
    {
        public event EventHandler<IHasProgress.ProgressChangedEventArgs> OnProgressChanged;
        public event EventHandler<StoveCounterStateChangeEventArgs> OnStoveCounterStateChange;

        public class StoveCounterStateChangeEventArgs : EventArgs
        {
            public StoveCounterState stoveCounterState;
        }

        public enum StoveCounterState
        {
            Empty,
            Cooking,
            Done,
            Burned
        }

        [SerializeField] private CookingRecipeSO[] cookingRecipeSOArray;
        [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

        private NetworkVariable<float> cookingTimer = new NetworkVariable<float>(0f);
        private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
        private NetworkVariable<StoveCounterState> stoveCounterState = new NetworkVariable<StoveCounterState>(StoveCounterState.Empty);

        private CookingRecipeSO cookingRecipeSO;
        private BurningRecipeSO burningRecipeSO;

        private void Start()
        {
            stoveCounterState.Value = StoveCounterState.Empty;
        }

        public override void OnNetworkSpawn()
        {
            cookingTimer.OnValueChanged += CookingTimer_OnValueChanged;
            burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
            stoveCounterState.OnValueChanged += StoveCounterState_OnValueChanged;
        }

        private void CookingTimer_OnValueChanged(float previousValue, float newValue)
        {
            float cookingTimerValue = cookingRecipeSO != null ? cookingRecipeSO.cookingTime : 1f;
            OnProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs { progressNormalized = cookingTimer.Value / cookingTimerValue });
        }

        private void BurningTimer_OnValueChanged(float previousValue, float newValue)
        {
            float burningTimerValue = burningRecipeSO != null ? burningRecipeSO.burningTime : 1f;
            OnProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs { progressNormalized = burningTimer.Value / burningTimerValue });
        }

        private void StoveCounterState_OnValueChanged(StoveCounterState previousValue, StoveCounterState newValue)
        {
            OnStoveCounterStateChange?.Invoke(this, new StoveCounterStateChangeEventArgs { stoveCounterState = stoveCounterState.Value });

            if (stoveCounterState.Value == StoveCounterState.Burned || stoveCounterState.Value == StoveCounterState.Empty)
            {
                OnProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs { progressNormalized = 0f });
            }
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }

            if (HasKitchenObject())
            {
                switch (stoveCounterState.Value)
                {
                    case StoveCounterState.Empty:
                        break;
                    case StoveCounterState.Cooking:
                        UpdateCooking();
                        break;
                    case StoveCounterState.Done:
                        if (TutorialManager.Instance != null)
                        {
                            return;
                        }
                        UpdateDone();
                        break;
                    case StoveCounterState.Burned:
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateCooking()
        {
            cookingTimer.Value += Time.deltaTime;

            if (cookingTimer.Value >= cookingRecipeSO.cookingTime)
            {
                // Cooking is done
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                KitchenObject.CreateKitchenObject(cookingRecipeSO.outputFoodObjectSO, this);
                stoveCounterState.Value = StoveCounterState.Done;
                burningTimer.Value = 0f;
                SetBurningRecipeSOClientRpc(EOSKitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                StepComplete();
            }
        }

        private void UpdateDone()
        {
            burningTimer.Value += Time.deltaTime;

            if (burningTimer.Value >= burningRecipeSO.burningTime)
            {
                // Burned
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                KitchenObject.CreateKitchenObject(burningRecipeSO.outputFoodObjectSO, this);
                stoveCounterState.Value = StoveCounterState.Burned;
            }
        }

        public override void Interact(Player player)
        {
            if (!HasKitchenObject())
            {
                // Player doesn't have a kitchen object
                if (player.HasKitchenObject())
                {
                    // Player has a kitchen object, so set it as this counter's kitchen object
                    if (HasCuttingRecipe(player.GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject kitchenObject = player.GetKitchenObject();
                        kitchenObject.SetKitchenObjectParent(this);

                        InteractServerRpc(EOSKitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
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
                            SetStoveCounterStateEmptyServerRpc();
                            StepComplete();
                        }
                    }
                }
                else
                {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    SetStoveCounterStateEmptyServerRpc();
                    StepComplete();
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetStoveCounterStateEmptyServerRpc()
        {
            stoveCounterState.Value = StoveCounterState.Empty;
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractServerRpc(int kitchenObjectSOIndex)
        {
            cookingTimer.Value = 0f;
            stoveCounterState.Value = StoveCounterState.Cooking;

            SetCookingRecipeSOClientRpc(kitchenObjectSOIndex);
        }

        [ClientRpc]
        private void SetCookingRecipeSOClientRpc(int kitchenObjectSOIndex)
        {
            KitchenObjectSO kitchenObjectSO = EOSKitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

            cookingRecipeSO = GetCookingRecipeSO(kitchenObjectSO);
        }

        [ClientRpc]
        private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
        {
            KitchenObjectSO kitchenObjectSO = EOSKitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

            burningRecipeSO = GetBurningRecipeSO(kitchenObjectSO);
        }

        private bool HasCuttingRecipe(KitchenObjectSO inputKitchenObjectSO)
        {
            return GetCookingRecipeSO(inputKitchenObjectSO) != null;
        }

        private KitchenObjectSO GetOutputKitchenObjectSO(KitchenObjectSO inputKitchenObjectSO)
        {
            CookingRecipeSO cookingRecipeSO = GetCookingRecipeSO(inputKitchenObjectSO);
            if (cookingRecipeSO != null)
            {
                return cookingRecipeSO.outputFoodObjectSO;
            }

            return null;
        }

        private CookingRecipeSO GetCookingRecipeSO(KitchenObjectSO inputKitchenObjectSO)
        {
            foreach (CookingRecipeSO cookingRecipeSO in cookingRecipeSOArray)
            {
                if (cookingRecipeSO.inputFoodObjectSO == inputKitchenObjectSO)
                {
                    return cookingRecipeSO;
                }
            }
            return null;
        }

        private BurningRecipeSO GetBurningRecipeSO(KitchenObjectSO inputKitchenObjectSO)
        {
            foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
            {
                if (burningRecipeSO.inputFoodObjectSO == inputKitchenObjectSO)
                {
                    return burningRecipeSO;
                }
            }
            return null;
        }

        public bool IsDone()
        {
            return stoveCounterState.Value == StoveCounterState.Done;
        }
    }
}
