using System.Threading.Tasks;

namespace RecipeRage.Managers.States
{
    public interface IGameStateHandler
    {
        void EnterState();
        void UpdateState();
        void ExitState();
    }
}