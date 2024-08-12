using RecipeRage.NakamaServer;
using UnityEngine;

namespace RecipeRage.Managers.States
{
    public class MainMenuStateHandler : GameStateHandlerBase
    {
        public MainMenuStateHandler(GameManager gameManager) : base(gameManager)
        {
        }

        public override void EnterState()
        {
            UIManager.Instance.ShowLoadingScreen();
            // ConnectToServer();
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            UIManager.Instance.HideMainMenuScreen();
        }
        
        private async void ConnectToServer()
        {
            
#if NAKAMA_MULTIPLAYER
            NakamaManager.Instance.Initialize(GameManager.NakamaConnectionData);
            await NakamaManager.Instance.ConnectToServer();
#endif
            GameSettingsManager.LoadGameSettings();
            GameModeManager.LoadGameMode();
            UIManager.Instance.HideLoadingScreen();
            UIManager.Instance.ShowMainMenuScreen();
        }
    }
}