using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class PlateKitchenObject : KitchenObject
    {
        public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
        public class OnIngredientAddedEventArgs : EventArgs
        {
            public KitchenObjectSO ingredient;
        }

        [SerializeField] private List<KitchenObjectSO> validIngredients;
        private List<KitchenObjectSO> ingredients;

        protected override void Awake()
        {
            base.Awake();
            ingredients = new List<KitchenObjectSO>();
        }

        // Returns true if ingredient was added to plate successfully and false if not (e.g. if ingredient is not valid for plate or same ingredient already exists on plate etc.)
        public bool TryAddIngredient(KitchenObjectSO ingredient)
        {
            if (validIngredients.Contains(ingredient) && !ingredients.Contains(ingredient))
            {
                AddIngredientServerRpc(MultiplayerManager.Instance.GetKitchenObjectSOIndex(ingredient));

                return true;
            }
            else
            {
                return false;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddIngredientServerRpc(int kitchenObjectSOIndex)
        {
            AddIngredientClientRpc(kitchenObjectSOIndex);
        }

        [ClientRpc]
        private void AddIngredientClientRpc(int kitchenObjectSOIndex)
        {
            KitchenObjectSO ingredient = MultiplayerManager.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

            ingredients.Add(ingredient);

            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { ingredient = ingredient });

        }

        public List<KitchenObjectSO> TryGetIngredient()
        {
            return ingredients;
        }
    }
}