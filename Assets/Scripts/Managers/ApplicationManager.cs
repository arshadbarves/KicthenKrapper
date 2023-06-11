using PlayEveryWare.EpicOnlineServices;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance { get; private set; }

    [SerializeField] private MainMenuUI m_mainMenuUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayerDataStorage.Instance.GetPlayerData(OnGetPlayerData);
    }

    private void OnGetPlayerData(string playerData)
    {
        if (playerData != null)
        {
            GameDataSource.Instance.SetPlayerData(JsonUtility.FromJson<PlayerDataInventory>(playerData));
            Debug.Log("[ApplicationManager]: Player Data Found: " + playerData);
        }
        else
        {
            Debug.Log("[ApplicationManager]: Player data not found. Creating new player data.");
            m_mainMenuUI.ShowPopupNamePanel();
        }
    }

    public void CreatePlayerData(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("[ApplicationManager]: Player name is null or empty.");
            return;
        }

        Debug.Log("[ApplicationManager]: Creating Player Account: " + EOSManager.Instance.GetProductUserId().ToString());
        PlayerDataInventory playerDataInventory = new PlayerDataInventory();
        playerDataInventory.PlayerId = EOSManager.Instance.GetProductUserId().ToString();
        playerDataInventory.PlayerName = playerName;

        PlayerDataStorage.Instance.CreatePlayerData(playerDataInventory);
        PlayerDataStorage.Instance.GetPlayerData(OnGetPlayerData);
    }
}
