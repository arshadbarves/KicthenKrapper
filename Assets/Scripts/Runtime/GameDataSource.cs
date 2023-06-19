using System;
using UnityEngine;

public class GameDataSource : MonoBehaviour
{
    public static GameDataSource Instance { get; private set; }

    public event EventHandler OnPlayerDataChanged;

    public static bool PlayMultiplayer { get; set; } = true;
    public static bool PlayTutorial { get; set; } = false;

    [SerializeField] private InputType inputType = InputType.Mobile;

    public struct GameSettings
    {
        public bool isMusicOn;
        public bool isSoundOn;
        public bool isVibrationOn;
        public bool isNotificationsOn;
        public float musicVolume;
        public float soundEffectsVolume;
    }

    public GameSettings gameSettings;

    private PlayerDataInventory playerData;
    private int deliveryCount;
    private int trophyCount;
    private int highestTrophyCount;
    private MapType currentMap;
    private GameObject player;
    private GameObject playerSkin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        gameSettings.isMusicOn = ClientPrefs.GetMusicToggle();
        gameSettings.isSoundOn = ClientPrefs.GetSoundEffectsToggle();
        gameSettings.isVibrationOn = false;
        gameSettings.isNotificationsOn = true;
        gameSettings.musicVolume = 0.5f;
        gameSettings.soundEffectsVolume = 0.5f;
    }

    public PlayerDataInventory GetPlayerData()
    {
        return playerData;
    }

    public void SetPlayerData(PlayerDataInventory data)
    {
        playerData = data;
        OnPlayerDataChanged?.Invoke(this, EventArgs.Empty);
    }

    public InputType GetInputType()
    {
        return inputType;
    }

    public void SetDeliveryCount(int count)
    {
        deliveryCount = count;
    }

    public int GetDeliveryCount()
    {
        return deliveryCount;
    }

    public void SetTrophyCount(int count)
    {
        trophyCount = count;
    }

    public int GetTrophyCount()
    {
        return trophyCount;
    }

    public void SetHighestTrophyCount(int count)
    {
        highestTrophyCount = count;
    }

    public int GetHighestTrophyCount()
    {
        return highestTrophyCount;
    }

    public void SetCurrentMap(MapType map)
    {
        currentMap = map;
    }

    public MapType GetCurrentMap()
    {
        return currentMap;
    }

    public void SetPlayer(GameObject playerObj)
    {
        player = playerObj;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void SetPlayerSkin(GameObject skin)
    {
        playerSkin = skin;
    }

    public GameObject GetPlayerSkin()
    {
        return playerSkin;
    }

    public void ResetGameData()
    {
        deliveryCount = 0;
        trophyCount = 0;
        highestTrophyCount = 0;
        currentMap = MapType.City;
        player = null;
        playerSkin = null;
    }
}
