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
        public Recipe Recipe { get; set; }
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
        if (!IsServer || !LevelManager.Instance.IsPlaying() || LevelManager.Instance.GetGameMode() == GameMode.Tutorial)
            return;

        SpawnRecipe();
        UpdateRecipeDeliveryTime();
    }

    private void SpawnRecipe()
    {
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f && waitingRecipeList.Count < waitingRecipeMaxCount)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            int randomIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
            int recipeID = RecipeIDGenerator.GetRecipeID();
            float deliveryTime = LevelManager.Instance.GetRecipeDeliveryTime();

            SpawnRecipeClientRpc(randomIndex, recipeID, deliveryTime);
        }
    }

    [ClientRpc]
    private void SpawnRecipeClientRpc(int randomIndex, int recipeID, float deliveryTime)
    {
        Recipe recipe = new Recipe(recipeID, deliveryTime, recipeListSO.recipeSOList[randomIndex]);
        waitingRecipeList.Add(recipeID, recipe);
        OnRecipeSpawned?.Invoke(this, new RecipeEventArgs { Recipe = recipe });
    }

    private void UpdateRecipeDeliveryTime()
    {
        foreach (Recipe recipe in waitingRecipeList.Values)
        {
            float deliveryTime = recipe.GetDeliveryTime() - Time.deltaTime;
            recipe.SetDeliveryTime(deliveryTime);

            if (deliveryTime <= 0f)
            {
                int recipeID = recipe.GetRecipeID();
                RemoveExpiredRecipeClientRpc(recipeID);
                return;
            }
        }
    }

    [ClientRpc]
    public void RemoveExpiredRecipeClientRpc(int recipeID)
    {
        expiredDeliveryCount++;
        OnRecipeExpired?.Invoke(this, new RecipeEventArgs { Recipe = waitingRecipeList[recipeID] });
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
        OnRecipeDelivered?.Invoke(this, new RecipeEventArgs { Recipe = waitingRecipeList[recipeID] });
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
                List<KitchenObjectSO> plateIngredients = plateKitchenObject.TryGetIngredient();

                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    if (!plateIngredients.Contains(recipeKitchenObjectSO))
                    {
                        plateContentsMatchRecipe = false;
                        break;
                    }
                }

                if (plateContentsMatchRecipe)
                {
                    DeliveryCorrectRecipeServerRpc(recipe.GetRecipeID());
                    return;
                }
            }
        }

        DeliveryIncorrectRecipeServerRpc();
    }

    public Dictionary<int, Recipe> GetWaitingRecipeList()
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
