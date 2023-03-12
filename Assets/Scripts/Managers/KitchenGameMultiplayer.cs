using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYERS = 4;
    public static KitchenGameMultiplayer Instance { get; private set; }



    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private NetworkList<PlayerData> playerDataNetworkList;

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

        playerDataNetworkList = new NetworkList<PlayerData>();
    }

    private void Start()
    {
        if (!GameDataSource.playMultiplayer)
        {
            SceneLoaderWrapper.Instance.LoadScene(SceneType.Map_City_001.ToString(), false);
        }
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
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        StopHost();
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void StopHost()
    {
        // Unsubscribe from events
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        StopClient();
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;

        NetworkManager.Singleton.StartClient();
    }

    public void StopClient()
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
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData { clientId = clientId });
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        // Get Active Scene
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
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
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
