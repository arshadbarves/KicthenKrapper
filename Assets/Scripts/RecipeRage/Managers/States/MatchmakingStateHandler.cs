namespace RecipeRage.Managers.States
{
    public class MatchmakingStateHandler: GameStateHandlerBase
    {
        public MatchmakingStateHandler(GameManager gameManager) : base(gameManager)
        {
        }

        public override void EnterState()
        {
            UIManager.Instance.ShowLoadingScreen();
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
            UIManager.Instance.HideLoadingScreen();
        }
    }
}