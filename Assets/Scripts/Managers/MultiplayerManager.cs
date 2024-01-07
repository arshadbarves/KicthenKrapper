using System;
using Epic.OnlineServices;
using Epic.OnlineServices.Sessions;
using KitchenKrapper;
using Multiplayer.EOS;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using PlayEveryWare.EpicOnlineServices.Samples.Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

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
        public event Action OnJoiningGame;
        public event Action OnFailedToJoinGame;
        public event Action OnPlayerDataNetworkListChanged;

        [FormerlySerializedAs("kitchenObjectListSO")] [SerializeField] private KitchenObjectListSO kitchenObjectListSo;

        private EOSTransportManager _transportManager;
        private readonly NetworkList<PlayerData> _sessionPlayers = new();
        private MultiplayerStatus _multiplayerStatus = MultiplayerStatus.NotConnected;

        private void Start()
        {
            _transportManager = EOSManager.Instance.GetOrCreateManager<EOSTransportManager>();
            _sessionPlayers.OnListChanged += PlayerDataNetworkList_OnListChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            _sessionPlayers.OnListChanged -= PlayerDataNetworkList_OnListChanged;

            _transportManager?.Disconnect();
        }

        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            OnPlayerDataNetworkListChanged?.Invoke();
        }

        public void StartHost(Action<bool> callback = null)
        {
            if (_multiplayerStatus == MultiplayerStatus.Hosting)
            {
                Debug.LogError("MultiplayerManager (StartHost): Unable to start hosting a session. A session is already being hosted.");
                callback?.Invoke(false);
                return;
            }

            if (_transportManager.StartHost())
            {
                _multiplayerStatus = MultiplayerStatus.Hosting;
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
            if (_multiplayerStatus == MultiplayerStatus.Joined)
            {
                Debug.LogError("MultiplayerManager (StartClient): Already joined a game session. You cannot join multiple sessions simultaneously.");
                callback?.Invoke(false);
                return;
            }
            OnJoiningGame?.Invoke();
            // PlayerController.SetNetworkHostId(hostId);
            if (_transportManager.StartClient())
            {
                _multiplayerStatus = MultiplayerStatus.Joined;
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
            var joinString = JsonUtility.ToJson(joinData);
            // Set the join information for the session
            EOSSessionsManager.SetJoinInfo(joinString);
        }
        public void Disconnect()
        {
            if (_multiplayerStatus != MultiplayerStatus.Hosting && _multiplayerStatus != MultiplayerStatus.Joined)
            {
                Debug.LogError("MultiplayerManager (Disconnect): Cannot disconnect - Not hosting or joined to a session.");
                return;
            }
            // Disconnect from the current session
            _transportManager?.Disconnect();
            _multiplayerStatus = MultiplayerStatus.NotConnected;
            EOSSessionsManager.SetJoinInfo(null);
        }
        private void OnDisconnect(ulong clientId)
        {
            Debug.LogWarning("MultiplayerManager (OnDisconnect): Disconnected from the session.");
            _multiplayerStatus = MultiplayerStatus.NotConnected;
            EOSSessionsManager.SetJoinInfo(null);
            // Remove the disconnected player from the session
            foreach (var playerData in _sessionPlayers)
            {
                if (playerData.clientId != clientId) continue;
                _sessionPlayers.Remove(playerData);
                break;
            }
            if (_multiplayerStatus == MultiplayerStatus.Hosting)
            {
                UnregisterHostCallbacks();
            }
            else
            {
                UnregisterClientCallbacks();
            }
        }

        private void RegisterHostCallbacks()
        {
            UnregisterHostCallbacks();
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        }

        private void UnregisterHostCallbacks()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
        }

        private void RegisterClientCallbacks()
        {
            OnJoiningGame?.Invoke();
            UnregisterClientCallbacks();

            // Subscribe to events
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        }

        private void UnregisterClientCallbacks()
        {
            // Unsubscribe from events
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Client_OnClientConnectedCallback;
        }

        private void NetworkManager_Server_OnClientDisconnectCallback(ulong playerID)
        {
            foreach (var playerData in _sessionPlayers)
            {
                if (playerData.clientId != playerID) continue;
                _sessionPlayers.Remove(playerData);
                break;
            }
            OnDisconnect(playerID);
        }

        private void NetworkManager_OnClientConnectedCallback(ulong clientId)
        {
            _sessionPlayers.Add(new PlayerData { clientId = clientId, playerName = GameManager.Instance.GetPlayerName() });
        }

        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            var session = SessionManager.Instance.GetCurrentSession();
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
        private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
        {
            var playerIndex = GetPlayerDataIndexFromPlayerId(serverRpcParams.Receive.SenderClientId);
            var playerData = _sessionPlayers[playerIndex];
            playerData.playerId = playerId;
            _sessionPlayers[playerIndex] = playerData;
        }

        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
        {
            OnFailedToJoinGame?.Invoke();
            OnDisconnect(clientId);
        }

        public void CreateKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent kitchenObjectParent)
        {
            CreateKitchenObjectServerRpc(GetKitchenObjectSoIndex(kitchenObjectSo), kitchenObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreateKitchenObjectServerRpc(int kitchenObjectSoIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
        {
            var kitchenObjectSo = GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);


            kitchenObjectParentNetworkObjectReference.TryGet(out var kitchenObjectParentNetworkObject);
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

            if (kitchenObjectParent.HasKitchenObject())
            {
                // Parent already has a kitchen object
                return;
            }

            var kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);

            var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
            kitchenObjectNetworkObject.Spawn(true);

            var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        }

        public int GetKitchenObjectSoIndex(KitchenObjectSO kitchenObjectSo)
        {
            return kitchenObjectListSo.kitchenObjectSOList.IndexOf(kitchenObjectSo);
        }

        public KitchenObjectSO GetKitchenObjectSoFromIndex(int kitchenObjectSoIndex)
        {
            return kitchenObjectListSo.kitchenObjectSOList[kitchenObjectSoIndex];
        }

        public void DestroyKitchenObject(KitchenObject kitchenObject)
        {
            DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
        {
            kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);

            if (kitchenObjectNetworkObject == null)
            {
                // This is a hack to fix a bug where the kitchen object is destroyed twice
                return;
            }

            var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
            RemoveKitchenObjectParentClientRpc(kitchenObjectNetworkObjectReference);
            kitchenObject.DestroySelf();
        }

        [ClientRpc]
        private void RemoveKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
        {
            kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);
            var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
            kitchenObject.RemoveKitchenObjectParent();
        }

        public PlayerData GetPlayerDataFromPlayerId(ulong playerID)
        {
            foreach (var playerData in _sessionPlayers)
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
            return _sessionPlayers;
        }

        private PlayerData GetPlayerData()
        {
            return GetPlayerDataFromPlayerId(NetworkManager.Singleton.LocalClientId);
        }

        public void SetPlayerReady(PlayerGameState playerGameState)
        {
            var playerData = GetPlayerData();
            playerData.playerGameState = playerGameState;
            SetPlayerDataServerRpc(playerData);
        }

        private int GetPlayerDataIndexFromPlayerId(ulong playerID)
        {
            for (var i = 0; i < _sessionPlayers.Count; i++)
            {
                if (_sessionPlayers[i].clientId == playerID)
                {
                    return i;
                }
            }

            return -1;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerDataServerRpc(PlayerData playerData, ServerRpcParams serverRpcParams = default)
        {
            var playerDataToChange = GetPlayerDataFromPlayerId(serverRpcParams.Receive.SenderClientId);
            playerDataToChange.playerGameState = playerData.playerGameState;

            var playerDataIndex = GetPlayerDataIndexFromPlayerId(serverRpcParams.Receive.SenderClientId);
            _sessionPlayers[playerDataIndex] = playerDataToChange;
        }

        public void KickPlayer(ulong playerID)
        {
            NetworkManager.Singleton.DisconnectClient(playerID);
            NetworkManager_Server_OnClientDisconnectCallback(playerID);
        }
    }
}