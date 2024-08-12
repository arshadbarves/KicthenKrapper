namespace RecipeRage.Managers.States
{
    public abstract class GameStateHandlerBase : IGameStateHandler
    {
        protected readonly GameManager GameManager;

        protected GameStateHandlerBase(GameManager gameManager)
        {
            GameManager = gameManager;
        }

        public abstract void EnterState();

        public abstract void UpdateState();

        public abstract void ExitState();
    }
}