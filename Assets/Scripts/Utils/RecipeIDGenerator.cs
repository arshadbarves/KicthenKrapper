

using UnityEngine;

public class RecipeIDGenerator : MonoBehaviour
{
    private static int recipeID = 0;

    public static int GetRecipeID()
    {
        return recipeID++;
    }
}
