using RecipeRage.UI.Controllers;

namespace RecipeRage.Managers.States
{
    public class SplashStateHandler : GameStateHandlerBase
    {
        public SplashStateHandler(GameManager gameManager) : base(gameManager)
        {
        }

        public override void EnterState()
        {
            UIManager.Instance.ShowSplashScreen();
            SplashUIController.OnSplashPanelActiveTransitionEnd += OnSplashPanelActiveTransitionEndHandler;
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
            UIManager.Instance.HideSplashScreen();
        }
        
        private void OnSplashPanelActiveTransitionEndHandler()
        {
            SplashUIController.OnSplashPanelActiveTransitionEnd -= OnSplashPanelActiveTransitionEndHandler;
            GameManager.Instance.ChangeGameState(GameState.MainMenu);
        }
    }
}