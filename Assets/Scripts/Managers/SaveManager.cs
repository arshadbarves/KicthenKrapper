using UnityEngine;

namespace KitchenKrapper
{
    [RequireComponent(typeof(GameManager))]
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string GAME_DATA_KEY = "GameData";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            GameManager.GameDataUpdated += SaveGame;
        }

        private void OnDestroy()
        {
            GameManager.GameDataUpdated -= SaveGame;
        }

        void OnApplicationQuit()
        {
            SaveGame();
        }

        public void SaveGame()
        {
            PlayerPrefs.SetString(GAME_DATA_KEY, GameManager.Instance.GameData.ToJson());
        }

        public void LoadGame()
        {
            if (HasSave())
            {
                print("Loading game data" + PlayerPrefs.GetString(GAME_DATA_KEY));
                GameManager.Instance.GameData = GameData.FromJson(PlayerPrefs.GetString(GAME_DATA_KEY));
            }
            else
            {
                GameManager.Instance.GameData = NewGame();
                print("Creating new game data" + GameManager.Instance.GameData.ToJson());
            }
        }

        private void DeleteSave()
        {
            PlayerPrefs.DeleteKey(GAME_DATA_KEY);
        }

        private bool HasSave()
        {
            return PlayerPrefs.HasKey(GAME_DATA_KEY);
        }

        public void ResetGame()
        {
            DeleteSave();
        }

        public GameData NewGame()
        {
            return new GameData();
        }
    }
}