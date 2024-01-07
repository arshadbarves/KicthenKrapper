using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    public class PlateIconsUI : MonoBehaviour
    {
        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private Transform iconTemplate;

        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);
        }

        private void Start()
        {
            plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        }

        private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            foreach (Transform child in transform)
            {
                if (child != iconTemplate)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (KitchenObjectSO ingredient in plateKitchenObject.TryGetIngredient())
            {
                Transform iconTransform = Instantiate(iconTemplate, transform);
                iconTransform.gameObject.SetActive(true);
                iconTransform.GetComponent<PlateIconSingleUI>().SetIcon(ingredient);
            }
        }
    }
}