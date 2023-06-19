using System;
using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using PlayEveryWare.EpicOnlineServices.Samples.Network;
using Unity.Netcode;
using UnityEngine;

public class P2PTransportPresenceData
{
    public const string ValidIdentifier = "P2PTRANSPORT";

    public string SceneIdentifier;
    public string ServerUserId;

    public bool IsValid()
    {
        return SceneIdentifier == ValidIdentifier;
    }
}

public class EOSKitchenGameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYERS = 4;
    public static EOSKitchenGameMultiplayer Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private EOSTransportManager transportManager = null;
    private NetworkList<PlayerData> playerDataNetworkList = new NetworkList<PlayerData>();
    private bool isHost = false;
    private bool isClient = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!GameDataSource.PlayMultiplayer)
        {
            SceneLoaderWrapper.Instance.LoadScene(SceneType.Map_City_001.ToString(), false);
        }
        if (!ClientPrefs.GetTutorialCompleted())
        {
            SceneLoaderWrapper.Instance.LoadScene(SceneType.Tutorial.ToString(), false);
            // StartHost();

        }
        transportManager = EOSManager.Instance.GetOrCreateManager<EOSTransportManager>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        playerDataNetworkList.OnListChanged -= PlayerDataNetworkList_OnListChanged;

        if (Instance == this)
        {
            Instance = null;
        }

        transportManager?.Disconnect();

        Player.DestroyNetworkManager();
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        if (isHost)
        {
            Debug.LogError("UIP2PTransportMenu (StartHostOnClick): already hosting");
            return;
        }

        if (transportManager.StartHost())
        {
            isHost = true;
            RegisterHostCallbacks();
            if (!ClientPrefs.GetTutorialCompleted()) return;
            SetJoinInfo(EOSManager.Instance.GetProductUserId());
            SceneLoaderWrapper.Instance.LoadScene(SceneType.Lobby.ToString(), true);
        }
        else
        {
            Debug.LogError("UIP2PTransportMenu (StartHostOnClick): failed to start host");
        }
    }

    public void StartClient(ProductUserId hostId)
    {
        if (hostId.IsValid())
        {
            if (isClient)
            {
                Debug.LogError("UIP2PTransportMenu (StartClientOnClick): already client");
                return;
            }
            OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
            Player.SetNetworkHostId(hostId);
            if (transportManager.StartClient())
            {
                RegisterCallbacks();
                isClient = true;
                // SetJoinInfo(hostId);
            }
            else
            {
                Debug.LogError("UIP2PTransportMenu (StartClientOnClick): failed to start client");
            }
        }
        else
        {
            Debug.LogError("UIP2PTransportMenu (JoinGame): invalid server user id");
        }
    }

    private void SetJoinInfo(ProductUserId serverUserId)
    {
        var joinData = new P2PTransportPresenceData()
        {
            SceneIdentifier = P2PTransportPresenceData.ValidIdentifier,
            ServerUserId = serverUserId.ToString()
        };

        string joinString = JsonUtility.ToJson(joinData);

        EOSSessionsManager.SetJoinInfo(joinString);
    }

    public void DisconnectOnClick()
    {
        transportManager?.Disconnect();
        isHost = false;
        isClient = false;
        EOSSessionsManager.SetJoinInfo(null);
    }

    private void OnDisconnect(ulong clientId)
    {
        Debug.LogWarning("UIP2PTransportMenu (OnDisconnect): server disconnected");
        isClient = false;
        EOSSessionsManager.SetJoinInfo(null);
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                playerDataNetworkList.Remove(playerData);
                break;
            }
        }
        if (isHost)
        {
            UnregisterHostCallbacks();
        }
        else
        {
            UnregisterCallbacks();
        }
    }

    public void RegisterHostCallbacks()
    {
        UnregisterHostCallbacks();
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
    }

    public void UnregisterHostCallbacks()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
    }

    public void RegisterCallbacks()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        UnregisterCallbacks();
        // Subscribe to events
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
    }

    public void UnregisterCallbacks()
    {
        // Unsubscribe from events
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong playerID)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == playerID)
            {
                playerDataNetworkList.Remove(playerData);
                break;
            }
        }
        OnDisconnect(playerID);
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData { clientId = clientId, playerName = GameDataSource.Instance.GetPlayerData().PlayerName });
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        // Get Active Scene Name and check if it is Lobby Scene Name or not
        if (SceneLoaderWrapper.Instance.GetActiveSceneName() != SceneType.Lobby.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is already in progress";
            return;
        }

        // Check if player count is less than max player count
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= MAX_PLAYERS)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Server is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerIdServerRpc(EOSManager.Instance.GetLocalUserId().ToString());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        throw new NotImplementedException();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerIndex = GetPlayerDataIndexFromPlayerId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerIndex];
        playerData.playerId = playerId;
        playerDataNetworkList[playerIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
        OnDisconnect(clientId);
    }

    public void CreateKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        CreateKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);


        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (kitchenObjectParent.HasKitchenObject())
        {
            // Parent already has a kitchen object
            return;
        }

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);

        if (kitchenObjectNetworkObject == null)
        {
            // This is a hack to fix a bug where the kitchen object is destroyed twice
            return;
        }

        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        RemoveKitchenObjectParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void RemoveKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.RemoveKitchenObjectParent();
    }

    public PlayerData GetPlayerDataFromPlayerId(ulong playerID)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == playerID)
            {
                return playerData;
            }
        }

        return default;
    }

    public NetworkList<PlayerData> GetPlayerDataNetworkList()
    {
        return playerDataNetworkList;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromPlayerId(NetworkManager.Singleton.LocalClientId);
    }

    public void SetPlayerReady(bool isReady)
    {
        PlayerData playerData = GetPlayerData();
        playerData.isReady = isReady;
        SetPlayerDataServerRpc(playerData);
    }

    public int GetPlayerDataIndexFromPlayerId(ulong playerID)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == playerID)
            {
                return i;
            }
        }

        return -1;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerDataServerRpc(PlayerData playerData, ServerRpcParams serverRpcParams = default)
    {
        PlayerData playerDataToChange = GetPlayerDataFromPlayerId(serverRpcParams.Receive.SenderClientId);
        playerDataToChange.isReady = playerData.isReady;

        int playerDataIndex = GetPlayerDataIndexFromPlayerId(serverRpcParams.Receive.SenderClientId);
        playerDataNetworkList[playerDataIndex] = playerDataToChange;
    }

    public void KickPlayer(ulong playerID)
    {
        NetworkManager.Singleton.DisconnectClient(playerID);
        NetworkManager_Server_OnClientDisconnectCallback(playerID);
    }
}
