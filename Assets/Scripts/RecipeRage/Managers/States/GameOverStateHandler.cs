using System.Threading.Tasks;

namespace RecipeRage.Managers.States
{
    public class GameOverStateHandler: GameStateHandlerBase
    {
        public GameOverStateHandler(GameManager gameManager) : base(gameManager)
        {
        }

        public override void EnterState()
        {
            UIManager.Instance.ShowGameOverScreen();
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            UIManager.Instance.HideGameOverScreen();
        }
    }
}