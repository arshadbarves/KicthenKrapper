using System.Collections;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameStatus gameStatus = GameStatus.None;

    public GameStatus GameStatus
    {
        get { return gameStatus; }
        set { gameStatus = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeApplication();
    }

    public void InitializeApplication()
    {
        UIManager.Instance.OnPanelShow(UIManager.Instance.TitlePanel);

        if (!ClientPrefs.GetEULAAndPrivacyPolicyAccepted())
        {
            // Show EULA panel in short delay to avoid blocking the UI.
            UIManager.Instance.OnPanelShowOnDelay(UIManager.Instance.EULAPanel, 1f);
            return;
        }
        else
        {
            StartApp();
        }
    }

    public void StartApp()
    {
        StartCoroutine(CheckInternetConnection());

        EOSAuth.Instance.Login();

        ClientPrefs.Initialize();
        GameDataSource.Instance.Initialize();
    }

    public void StartGame()
    {
        gameStatus = GameStatus.GameStarted;

        SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), false);

        UIManager.Instance.HideAllPanels();
    }

    public void QuitGame()
    {
        gameStatus = GameStatus.GameQuit;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UIManager.Instance.OnPanelShow(UIManager.Instance.NetworkErrorPanel);
        }
        else
        {
            UIManager.Instance.OnPanelHide(UIManager.Instance.NetworkErrorPanel);
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(CheckInternetConnection());
    }

    private void ResetPlayerAuth()
    {
        var authInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
        var options = new Epic.OnlineServices.Auth.DeletePersistentAuthOptions();

        authInterface.DeletePersistentAuth(ref options, null, (ref DeletePersistentAuthCallbackInfo deletePersistentAuthCallbackInfo) =>
        {
            if (deletePersistentAuthCallbackInfo.ResultCode == Result.Success)
            {
                Debug.Log("Persistent auth deleted");
            }
            else
            {
                Debug.Log("Persistent auth not deleted");
            }
        });

        var connectInterface = EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
        var connectOptions = new Epic.OnlineServices.Connect.DeleteDeviceIdOptions();

        connectInterface.DeleteDeviceId(ref connectOptions, null, (ref DeleteDeviceIdCallbackInfo deleteDeviceIdCallbackInfo) =>
        {
            if (deleteDeviceIdCallbackInfo.ResultCode == Result.Success)
            {
                Debug.Log("Device ID deleted");
            }
            else
            {
                Debug.Log("Device ID not deleted");
            }
        });
    }

    public void CleanupApplication()
    {
        GameDataSource.Instance.ResetGameData();
        gameStatus = GameStatus.None;
        ClientPrefs.ResetClientPrefs();
        ResetPlayerAuth();
        InitializeApplication();

        if (ApplicationManager.Instance != null)
        {
            Destroy(ApplicationManager.Instance.gameObject);
        }
    }

    public void PlayerDataStorageCallback(string playerData)
    {
        if (playerData != null)
        {
            PlayerDataInventory data = JsonUtility.FromJson<PlayerDataInventory>(playerData);
            GameDataSource.Instance.SetPlayerData(data);
            Debug.Log("[ApplicationManager]: Player Data Found: " + playerData);
        }
        else
        {
            Debug.Log("[ApplicationManager]: Player data not found. Creating new player data.");
            ApplicationManager.Instance.MainMenuUI.ShowPopupNamePanel();
        }
    }

    public void CreatePlayerData(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("[ApplicationManager]: Player name is null or empty.");
            return;
        }

        string playerId = EOSManager.Instance.GetProductUserId().ToString();
        Debug.Log("[ApplicationManager]: Creating Player Account: " + playerId);

        PlayerDataInventory playerDataInventory = new PlayerDataInventory
        {
            PlayerId = playerId,
            PlayerName = playerName
        };

        PlayerDataStorage.Instance.CreatePlayerData(playerDataInventory);
        PlayerDataStorage.Instance.GetPlayerData(PlayerDataStorageCallback);
    }
}
