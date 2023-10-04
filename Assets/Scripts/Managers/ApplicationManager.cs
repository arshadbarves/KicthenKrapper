using UnityEngine;

namespace KitchenKrapper
{
    public class ApplicationManager : MonoBehaviour
    {
        public static ApplicationManager Instance { get; private set; }

        [SerializeField] private MainMenuUI mainMenuUI;

        public MainMenuUI MainMenuUI { get { return mainMenuUI; } }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PlayerDataStorage.Instance.GetPlayerData(GameManager.Instance.PlayerDataStorageCallback);
        }
    }
}