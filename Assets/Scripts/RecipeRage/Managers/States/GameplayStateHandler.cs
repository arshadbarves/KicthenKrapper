using System.Threading.Tasks;

namespace RecipeRage.Managers.States
{
    public class GameplayStateHandler : IGameStateHandler
    {
        private readonly GameManager _gameManager;

        public GameplayStateHandler(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public async void EnterState()
        {
            await Task.CompletedTask;
        }

        public void UpdateState()
        {
        }

        public void ExitState()
        {
        }
    }
}