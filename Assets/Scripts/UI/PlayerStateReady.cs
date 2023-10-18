using System.Collections.Generic;
using Unity.Netcode;

namespace KitchenKrapper
{
    public class PlayerState : NetworkBehaviour
    {
        public static PlayerState Instance { get; private set; }

        private Dictionary<ulong, bool> playerReadyStates;
        private LobbyManager lobbyManager;

        private void Awake()
        {
            InitializeSingleton();
            InitializePlayerReadyStates();
        }

        private void Start()
        {
            lobbyManager = LobbyManager.Instance;
        }

        private void InitializeSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void InitializePlayerReadyStates()
        {
            playerReadyStates = new Dictionary<ulong, bool>();
        }

        private void TogglePlayerReadyState()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (!playerReadyStates.TryGetValue(localClientId, out bool isReady))
            {
                isReady = false;
            }

            SetPlayerReadyServerRpc(localClientId, !isReady);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ulong clientId, bool isReady = false, ServerRpcParams serverRpcParams = default)
        {
            playerReadyStates[clientId] = isReady;

            if (AllPlayersReady())
            {
                lobbyManager.StartMatchmaking();
            }
        }

        private bool AllPlayersReady()
        {
            foreach (var playerReady in playerReadyStates)
            {
                if (!playerReady.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPlayerReady()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;
            return playerReadyStates.TryGetValue(localClientId, out bool isReady) && isReady;
        }
    }
}
