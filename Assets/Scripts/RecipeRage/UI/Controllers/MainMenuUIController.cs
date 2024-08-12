using RecipeRage.Managers;
using RecipeRage.UI.Base;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecipeRage.UI.Controllers
{
    public class MainMenuUIController: UIScreen
    {
        private Label _playerNameLabel;
        private Label _playerTrophiesLabel;
        private Image _playerAvatarImage;
        private Button _playButton;
        private Button _leaderboardButton;
        private Button _profileButton;
        private Button _friendsButton;
        private Button _selectGameModeButton;
        
        protected override void Awake()
        {
            base.Awake();
            
            _playerNameLabel = Root.Q<Label>("playerNameLabel");
            _playerTrophiesLabel = Root.Q<Label>("playerTrophiesLabel");
            _playerAvatarImage = Root.Q<Image>("playerAvatarImage");
            _playButton = Root.Q<Button>("playButton");
            // _leaderboardButton = Root.Q<Button>("leaderboardButton");
            _profileButton = Root.Q<Button>("profileButton");
            // _friendsButton = Root.Q<Button>("friendsButton");
            _selectGameModeButton = Root.Q<Button>("selectGameModeButton");

            _playButton.clicked += OnPlayButtonClicked;
            _leaderboardButton.clicked += OnLeaderboardButtonClicked;
            _profileButton.clicked += OnProfileButtonClicked;
            _friendsButton.clicked += OnFriendsButtonClicked;
            _selectGameModeButton.clicked += OnSelectGameModeButtonClicked;
        }

        private void Start()
        {
            _playerNameLabel.text = "Player Name";
            _playerTrophiesLabel.text = "Player Trophies";
            _playerAvatarImage.image = null;
            
            GameModeManager.OnGameModeChanged += OnGameModeChanged;
        }

        private void OnGameModeChanged(string gameMode)
        {
            _selectGameModeButton.text = gameMode;
        }

        private void OnDestroy()
        {
            _playButton.clicked -= OnPlayButtonClicked;
        }

        private static void OnPlayButtonClicked()
        {
            GameManager.Instance.ChangeGameState(GameState.Matchmaking);
        }
        
        private static void OnLeaderboardButtonClicked()
        {
            Debug.Log("Leaderboard button clicked");
        }
        
        private static void OnProfileButtonClicked()
        {
            Debug.Log("Profile button clicked");
        }
        
        private static void OnFriendsButtonClicked()
        {
            Debug.Log("Friends button clicked");
        }
        
        private static void OnSelectGameModeButtonClicked()
        {
            Debug.Log("Select game mode button clicked");
        }
    }
}