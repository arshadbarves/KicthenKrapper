using System;
using Epic.OnlineServices;
using Epic.OnlineServices.Sessions;
using KitchenKrapper;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using PlayEveryWare.EpicOnlineServices.Samples.Network;
using Unity.Netcode;
using UnityEngine;

namespace Managers
{
    public enum MultiplayerStatus
    {
        NotConnected,   // Player is not connected to any session
        Hosting,        // Player is hosting a session
        Joined          // Player has joined a session
    }

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

    public class MultiplayerManager : NetworkSingleton<MultiplayerManager>
    {
        public event Action JoiningGame;
        public event Action FailedToJoinGame;
        public event Action PlayerDataNetworkListChanged;

        [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

        private EOSTransportManager transportManager = null;
        private NetworkList<PlayerData> sessionPlayers = new NetworkList<PlayerData>();
        private MultiplayerStatus multiplayerStatus = MultiplayerStatus.NotConnected;
        private bool isHosting = false;
        private bool isJoined = false;

        private void Start()
        {
            transportManager = EOSManager.Instance.GetOrCreateManager<EOSTransportManager>();
            sessionPlayers.OnListChanged += PlayerDataNetworkList_OnListChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            sessionPlayers.OnListChanged -= PlayerDataNetworkList_OnListChanged;

            transportManager?.Disconnect();

            PlayerController.DestroyNetworkManager();
        }

        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            PlayerDataNetworkListChanged?.Invoke();
        }

        public void StartHost(Action<bool> callback = null)
        {
            if (multiplayerStatus == MultiplayerStatus.Hosting)
            {
                Debug.LogError("MultiplayerManager (StartHost): Unable to start hosting a session. A session is already being hosted.");
                callback?.Invoke(false);
                return;
            }

            if (transportManager.StartHost())
            {
                multiplayerStatus = MultiplayerStatus.Hosting;
                RegisterHostCallbacks();
                SetJoinInfo(EOSManager.Instance.GetProductUserId());
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogError("MultiplayerManager (StartHost): Failed to initiate hosting a session. Check your network settings.");
                callback?.Invoke(false);
            }
        }

        public void StartClient(ProductUserId hostId, Action<bool> callback = null)
        {
            if (!hostId.IsValid())
            {
                Debug.LogError("MultiplayerManager (StartClient): Invalid server user ID. Unable to join the game.");
                callback?.Invoke(false);
                return;
            }
            if (multiplayerStatus == MultiplayerStatus.Joined)
            {
                Debug.LogError("MultiplayerManager (StartClient): Already joined a game session. You cannot join multiple sessions simultaneously.");
                callback?.Invoke(false);
                return;
            }
            JoiningGame?.Invoke();
            PlayerController.SetNetworkHostId(hostId);
            if (transportManager.StartClient())
            {
                multiplayerStatus = MultiplayerStatus.Joined;
                RegisterClientCallbacks();
                SetJoinInfo(hostId);
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogError("MultiplayerManager (StartClient): Failed to initiate joining the game. Please check your network connection.");
                callback?.Invoke(false);
            }
        }


        private void SetJoinInfo(ProductUserId serverUserId)
        {
            // Create the join data to communicate with the server
            var joinData = new P2PTransportPresenceData()
            {
                SceneIdentifier = P2PTransportPresenceData.ValidIdentifier,
                ServerUserId = serverUserId.ToString()
            };
            // Serialize the join data to JSON
            string joinString = JsonUtility.ToJson(joinData);
            // Set the join information for the session
            EOSSessionsManager.SetJoinInfo(joinString);
        }
        public void Disconnect()
        {
            if (multiplayerStatus != MultiplayerStatus.Hosting && multiplayerStatus != MultiplayerStatus.Joined)
            {
                Debug.LogError("MultiplayerManager (Disconnect): Cannot disconnect - Not hosting or joined to a session.");
                return;
            }
            // Disconnect from the current session
            transportManager?.Disconnect();
            multiplayerStatus = MultiplayerStatus.NotConnected;
            EOSSessionsManager.SetJoinInfo(null);
        }
        private void OnDisconnect(ulong clientId)
        {
            Debug.LogWarning("MultiplayerManager (OnDisconnect): Disconnected from the session.");
            multiplayerStatus = MultiplayerStatus.NotConnected;
            EOSSessionsManager.SetJoinInfo(null);
            // Remove the disconnected player from the session
            foreach (PlayerData playerData in sessionPlayers)
            {
                if (playerData.clientId == clientId)
                {
                    sessionPlayers.Remove(playerData);
                    break;
                }
            }
            if (multiplayerStatus == MultiplayerStatus.Hosting)
            {
                UnregisterHostCallbacks();
            }
            else
            {
                UnregisterClientCallbacks();
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

        public void RegisterClientCallbacks()
        {
            JoiningGame?.Invoke();
            UnregisterClientCallbacks();

            // Subscribe to events
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        }

        public void UnregisterClientCallbacks()
        {
            // Unsubscribe from events
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
        }

        private void NetworkManager_Server_OnClientDisconnectCallback(ulong playerID)
        {
            foreach (PlayerData playerData in sessionPlayers)
            {
                if (playerData.clientId == playerID)
                {
                    sessionPlayers.Remove(playerData);
                    break;
                }
            }
            OnDisconnect(playerID);
        }

        private void NetworkManager_OnClientConnectedCallback(ulong clientId)
        {
            sessionPlayers.Add(new PlayerData { clientId = clientId, playerName = GameManager.Instance.GetPlayerName() });
        }

        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            Session session = SessionManager.Instance.GetCurrentSession();
            // Check if game is already in progress
            if (session.SessionState == OnlineSessionState.InProgress)
            {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Game is already in progress";
                return;
            }

            // Check if player count is less than session max player count
            if (NetworkManager.Singleton.ConnectedClientsList.Count >= session.MaxPlayers)
            {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Game is full";
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
            PlayerData playerData = sessionPlayers[playerIndex];
            playerData.playerId = playerId;
            sessionPlayers[playerIndex] = playerData;
        }

        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
        {
            FailedToJoinGame?.Invoke();
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
            foreach (PlayerData playerData in sessionPlayers)
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
            return sessionPlayers;
        }

        public PlayerData GetPlayerData()
        {
            return GetPlayerDataFromPlayerId(NetworkManager.Singleton.LocalClientId);
        }

        public void SetPlayerReady(PlayerGameState playerGameState)
        {
            PlayerData playerData = GetPlayerData();
            playerData.playerGameState = playerGameState;
            SetPlayerDataServerRpc(playerData);
        }

        public int GetPlayerDataIndexFromPlayerId(ulong playerID)
        {
            for (int i = 0; i < sessionPlayers.Count; i++)
            {
                if (sessionPlayers[i].clientId == playerID)
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
            playerDataToChange.playerGameState = playerData.playerGameState;

            int playerDataIndex = GetPlayerDataIndexFromPlayerId(serverRpcParams.Receive.SenderClientId);
            sessionPlayers[playerDataIndex] = playerDataToChange;
        }

        public void KickPlayer(ulong playerID)
        {
            NetworkManager.Singleton.DisconnectClient(playerID);
            NetworkManager_Server_OnClientDisconnectCallback(playerID);
        }
    }
}