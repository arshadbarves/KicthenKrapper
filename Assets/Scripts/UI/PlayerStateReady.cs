using System.Collections.Generic;
using Unity.Netcode;

namespace KitchenKrapper
{
    public class PlayerStateReady : NetworkBehaviour
    {
        public static PlayerStateReady Instance { get; private set; }

        private Dictionary<ulong, bool> playerReadyDictionary;

        private void Awake()
        {
            InitializeSingleton();
            InitializePlayerReadyDictionary();
        }

        private void InitializeSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void InitializePlayerReadyDictionary()
        {
            playerReadyDictionary = new Dictionary<ulong, bool>();
        }

        public void OnReadyButtonClicked()
        {
            TogglePlayerReadyState();
        }

        private void TogglePlayerReadyState()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (!playerReadyDictionary.ContainsKey(localClientId))
            {
                playerReadyDictionary.Add(localClientId, false);
            }

            bool isReady = !playerReadyDictionary[localClientId];
            SetPlayerReadyServerRpc(localClientId, isReady);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ulong clientId, bool isReady = false, ServerRpcParams serverRpcParams = default)
        {
            playerReadyDictionary[clientId] = isReady;

            if (AreAllPlayersReady())
            {
                HandleAllPlayersReady();
            }
        }

        private bool AreAllPlayersReady()
        {
            foreach (var playerReady in playerReadyDictionary)
            {
                if (!playerReady.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private void HandleAllPlayersReady()
        {
            SceneLoaderWrapper.Instance.LoadScene(SceneType.Map_City_001.ToString(), true);
        }

        public bool IsReady()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;

            if (playerReadyDictionary.ContainsKey(localClientId))
            {
                return playerReadyDictionary[localClientId];
            }

            return false;
        }
    }
}
