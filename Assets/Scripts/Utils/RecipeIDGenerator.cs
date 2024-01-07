namespace Utils
{
    public static class RecipeIDGenerator
    {
        private static int _recipeID;

        public static int GetRecipeID()
        {
            return _recipeID++;
        }
    }
}