using System;
using Managers;
using UnityEngine;

namespace KitchenKrapper
{
    public class DeliveryManagerUI : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private Transform recipeTemplate;

        private void Awake()
        {
            recipeTemplate.gameObject.SetActive(false);
        }

        private void Start()
        {
            SubscribeToEvents();
            UpdateVisual();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            DeliveryManager.Instance.OnRecipeSpawned += OnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeDelivered += OnRecipeDelivered;
            DeliveryManager.Instance.OnRecipeExpired += OnRecipeExpired;
        }

        private void UnsubscribeFromEvents()
        {
            DeliveryManager.Instance.OnRecipeSpawned -= OnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeDelivered -= OnRecipeDelivered;
            DeliveryManager.Instance.OnRecipeExpired -= OnRecipeExpired;
        }

        private void OnRecipeSpawned(object sender, DeliveryManager.RecipeEventArgs e)
        {
            AddRecipe(e.Recipe);
        }

        private void OnRecipeExpired(object sender, DeliveryManager.RecipeEventArgs e)
        {
            RemoveRecipe(e.Recipe);
        }

        private void OnRecipeDelivered(object sender, DeliveryManager.RecipeEventArgs e)
        {
            RemoveRecipe(e.Recipe);
        }

        private void UpdateVisual()
        {
            ClearContainer();

            foreach (Recipe recipe in DeliveryManager.Instance.GetWaitingRecipeList().Values)
            {
                AddRecipe(recipe);
            }
        }

        private void ClearContainer()
        {
            foreach (Transform child in container)
            {
                if (child == recipeTemplate) continue;
                Destroy(child.gameObject);
            }
        }

        private void AddRecipe(Recipe recipe)
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipe(recipe);
        }

        private void RemoveRecipe(Recipe recipe)
        {
            foreach (Transform child in container)
            {
                if (child == recipeTemplate) continue;
                DeliveryManagerSingleUI recipeUI = child.GetComponent<DeliveryManagerSingleUI>();
                if (recipeUI.GetRecipe().GetRecipeID() == recipe.GetRecipeID())
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
    }
}