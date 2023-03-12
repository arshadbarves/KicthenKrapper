using System;
using UnityEngine;

public class GameDataSource : MonoBehaviour
{
    public static GameDataSource Instance { get; private set; }


    public static bool playMultiplayer = true;

    [SerializeField] private bool m_useNetworkSceneManager = false;
    [SerializeField] private InputType inputType = InputType.Mobile;

    private int m_DeliveryCount;
    private int m_TrophyCount;
    private int m_HighestTrophyCount;
    private MapType m_CurrentMap;
    private GameObject m_Player;
    private GameObject m_PlayerSkin;

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

    public InputType GetInputType()
    {
        return inputType;
    }

    public void SetDeliveryCount(int count)
    {
        m_DeliveryCount = count;
    }

    public int GetDeliveryCount()
    {
        return m_DeliveryCount;
    }

    public void SetTrophyCount(int count)
    {
        m_TrophyCount = count;
    }

    public int GetTrophyCount()
    {
        return m_TrophyCount;
    }

    public void SetHighestTrophyCount(int count)
    {
        m_HighestTrophyCount = count;
    }

    public int GetHighestTrophyCount()
    {
        return m_HighestTrophyCount;
    }

    public void SetCurrentMap(MapType map)
    {
        m_CurrentMap = map;
    }

    public MapType GetCurrentMap()
    {
        return m_CurrentMap;
    }

    public void SetPlayer(GameObject player)
    {
        m_Player = player;
    }

    public GameObject GetPlayer()
    {
        return m_Player;
    }

    public void SetPlayerSkin(GameObject playerSkin)
    {
        m_PlayerSkin = playerSkin;
    }

    public GameObject GetPlayerSkin()
    {
        return m_PlayerSkin;
    }

    public bool UseNetworkSceneManager()
    {
        return m_useNetworkSceneManager;
    }

    public void SetUseNetworkSceneManager(bool useNetworkSceneManager)
    {
        m_useNetworkSceneManager = useNetworkSceneManager;
    }

    public bool GetEULAAndPrivacyPolicyAccepted()
    {
        return PlayerPrefs.GetInt("EULAAndPrivacyPolicyAccepted", 0) == 1;
    }

    public void SetEULAAndPrivacyPolicyAccepted(bool accepted)
    {
        PlayerPrefs.SetInt("EULAAndPrivacyPolicyAccepted", accepted ? 1 : 0);
    }
}
