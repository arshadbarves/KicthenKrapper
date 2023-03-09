using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler<RecipeEventArgs> OnRecipeSpawned;
    public event EventHandler<RecipeEventArgs> OnRecipeDelivered;
    public event EventHandler OnRecipeDeliveryFailed;
    public event EventHandler<RecipeEventArgs> OnRecipeExpired;

    public class RecipeEventArgs : EventArgs
    {
        public Recipe recipe;
    }

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    [SerializeField] private float spawnRecipeTimerMax = 5f;
    [SerializeField] private int waitingRecipeMaxCount = 3;
    private Dictionary<int, Recipe> waitingRecipeList;
    private float spawnRecipeTimer = 4f;
    private int successfulDeliveryCount = 0;
    private int failedDeliveryCount = 0;
    private int expiredDeliveryCount = 0;

    private void Awake()
    {
        Instance = this;
        waitingRecipeList = new Dictionary<int, Recipe>();
    }

    private void Update()
    {
        if (!IsServer) return; // Only the server can spawn recipes

        SpawnRecipe();
        UpdateRecipeDeliveryTime();
    }

    private void SpawnRecipe()
    {
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (GameManager.Instance.IsPlaying() && waitingRecipeList.Count < waitingRecipeMaxCount)
            {
                int randomIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                SpawnRecipeClientRpc(randomIndex, RecipeIDGenerator.GetRecipeID(), GameManager.Instance.GetRecipeDeliveryTime());
            }
        }
    }

    [ClientRpc]
    private void SpawnRecipeClientRpc(int randomIndex, int recipeID, float deliveryTime)
    {
        Recipe recipe = new Recipe(recipeID, deliveryTime, recipeListSO.recipeSOList[randomIndex]);
        waitingRecipeList.Add(recipeID, recipe);
        OnRecipeSpawned?.Invoke(this, new RecipeEventArgs { recipe = recipe });
    }

    private void UpdateRecipeDeliveryTime()
    {
        foreach (Recipe recipe in waitingRecipeList.Values)
        {
            recipe.SetDeliveryTime(recipe.GetDeliveryTime() - Time.deltaTime);

            if (recipe.GetDeliveryTime() <= 0f)
            {
                RemoveExpiredRecipeClientRpc(recipe.GetRecipeID());
                return;
            }
        }
    }

    [ClientRpc]
    public void RemoveExpiredRecipeClientRpc(int recipeID)
    {
        expiredDeliveryCount++;
        OnRecipeExpired?.Invoke(this, new RecipeEventArgs { recipe = waitingRecipeList[recipeID] });
        RemoveRecipe(recipeID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryCorrectRecipeServerRpc(int recipeID)
    {
        DeliveryCorrectRecipeClientRpc(recipeID);
    }

    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int recipeID)
    {
        successfulDeliveryCount++;
        OnRecipeDelivered?.Invoke(this, new RecipeEventArgs { recipe = waitingRecipeList[recipeID] });
        RemoveRecipe(recipeID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryIncorrectRecipeServerRpc()
    {
        DeliveryIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliveryIncorrectRecipeClientRpc()
    {
        failedDeliveryCount++;
        OnRecipeDeliveryFailed?.Invoke(this, EventArgs.Empty);
    }

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
    {
        foreach (Recipe recipe in waitingRecipeList.Values)
        {
            RecipeSO waitingRecipeSO = recipe.GetRecipeSO();

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.TryGetIngredient().Count)
            {
                bool plateContentsMatchRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;

                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.TryGetIngredient())
                    {
                        if (recipeKitchenObjectSO == plateKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        // This is not the recipe you are looking for
                        plateContentsMatchRecipe = false;
                    }
                }
                if (plateContentsMatchRecipe)
                {
                    // This is the recipe you are looking for
                    DeliveryCorrectRecipeServerRpc(recipe.GetRecipeID());
                    return;
                }
            }
        }
        DeliveryIncorrectRecipeServerRpc();
    }

    public Dictionary<int, Recipe> GetWaitingRecipeSOList()
    {
        return waitingRecipeList;
    }

    public int GetSuccessfulDeliveryCount()
    {
        return successfulDeliveryCount;
    }

    public int GetFailedDeliveryCount()
    {
        return failedDeliveryCount;
    }

    public int GetExpiredDeliveryCount()
    {
        return expiredDeliveryCount;
    }

    public void RemoveRecipe(int recipeID)
    {
        waitingRecipeList.Remove(recipeID);
    }
}
