using UnityEngine;

public class Recipe
{
    private int recipeID;
    private RecipeSO recipeSO;
    private float deliveryTime;

    // For client
    public Recipe(int recipeID, float deliveryTime, RecipeSO recipeSO)
    {
        this.recipeID = recipeID;
        this.deliveryTime = deliveryTime;
        this.recipeSO = recipeSO;
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        this.recipeSO = recipeSO;
    }

    public void SetDeliveryTime(float deliveryTime)
    {
        this.deliveryTime = deliveryTime;
    }

    public void SetRecipeID(int recipeID)
    {
        this.recipeID = recipeID;
    }

    public RecipeSO GetRecipeSO()
    {
        return recipeSO;
    }

    public float GetDeliveryTime()
    {
        return deliveryTime;
    }

    public int GetRecipeID()
    {
        return recipeID;
    }
}
