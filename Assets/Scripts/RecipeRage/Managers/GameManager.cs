using System.Collections.Generic;
using RecipeRage.Managers.States;
using RecipeRage.Multiplayer.NakamaServer;

namespace RecipeRage.Managers
{
    public enum GameState
    {
        Splash,
        Loading,
        MainMenu,
        Matchmaking,
        Game,
        GameOver
    }

    // State machine for the game
    public class GameManager : MonoSingleton<GameManager>
    {
        private readonly Dictionary<GameState, IGameStateHandler> _gameStateHandlers =
            new Dictionary<GameState, IGameStateHandler>();

        private IGameStateHandler _currentStateHandler;

        public GameState CurrentGameState { get; private set; }
        
        public string CurrentGameModeMap { get; private set; }

#if NAKAMA_MULTIPLAYER
        public NakamaConnectionData NakamaConnectionData { get; private set; }
#endif

        protected override void Awake()
        {
            base.Awake();
            InitializeGameStateHandlers();
        }

        private void Start()
        {
#if NAKAMA_MULTIPLAYER
            NakamaConnectionData = Bootstrap.Instance.NakamaConnectionData;
#endif
            ChangeGameState(GameState.Splash);
        }

        private void Update()
        {
            _currentStateHandler?.UpdateState();
        }

        public void ChangeGameState(GameState gameState)
        {
            _currentStateHandler?.ExitState();

            CurrentGameState = gameState;
            _currentStateHandler = _gameStateHandlers[gameState];
            _currentStateHandler.EnterState();
        }

        private void InitializeGameStateHandlers()
        {
            _gameStateHandlers.Clear();
            _gameStateHandlers.Add(GameState.Splash, new SplashStateHandler(this));
            _gameStateHandlers.Add(GameState.Loading, new LoadingStateHandler(this));
            _gameStateHandlers.Add(GameState.MainMenu, new MainMenuStateHandler(this));
            _gameStateHandlers.Add(GameState.Matchmaking, new MatchmakingStateHandler(this));
            _gameStateHandlers.Add(GameState.Game, new GameplayStateHandler(this));
            _gameStateHandlers.Add(GameState.GameOver, new GameOverStateHandler(this));
        }
        
        public void SetGameModeMap(string gameModeMap)
        {
            CurrentGameModeMap = gameModeMap;
        }
    }
}