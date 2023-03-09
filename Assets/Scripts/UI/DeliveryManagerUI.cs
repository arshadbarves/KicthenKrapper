using System;
using UnityEngine;

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
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
        DeliveryManager.Instance.OnRecipeExpired += DeliveryManager_OnRecipeExpired;
        // UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, DeliveryManager.RecipeEventArgs e)
    {
        AddRecipe(e.recipe);
    }

    private void DeliveryManager_OnRecipeExpired(object sender, DeliveryManager.RecipeEventArgs e)
    {
        Debug.Log("Recipe expired" + e.recipe.GetRecipeSO().name);
        RemoveRecipe(e.recipe);
    }

    private void DeliveryManager_OnRecipeDelivered(object sender, DeliveryManager.RecipeEventArgs e)
    {
        RemoveRecipe(e.recipe);
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Recipe recipe in DeliveryManager.Instance.GetWaitingRecipeSOList().Values)
        {
            AddRecipe(recipe);
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
            if (child.GetComponent<DeliveryManagerSingleUI>().GetRecipe().GetRecipeID() == recipe.GetRecipeID())
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }
}
