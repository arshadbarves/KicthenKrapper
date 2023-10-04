using System;
using System.Collections.Generic;

using UnityEngine;

namespace KitchenKrapper
{
    public class PlateCompleteVisual : MonoBehaviour
    {
        [Serializable]
        public struct KitchenObjectSO_GameObject
        {
            public KitchenObjectSO kitchenObjectSO;
            public GameObject gameObject;
        }

        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjects;

        private void Start()
        {
            plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

            foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjects)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(false);
            }
        }

        private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
        {
            foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjects)
            {
                if (kitchenObjectSOGameObject.kitchenObjectSO == e.ingredient)
                {
                    kitchenObjectSOGameObject.gameObject.SetActive(true);
                }
            }
        }
    }
}