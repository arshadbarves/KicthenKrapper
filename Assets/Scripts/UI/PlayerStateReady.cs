using System.Collections.Generic;
using Unity.Netcode;

public class PlayerStateReady : NetworkBehaviour
{
    public static PlayerStateReady Instance { get; private set; }
    private Dictionary<ulong, bool> playerReadyDictionary;

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
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void OnReadyButtonClicked()
    {
        // Toggle ready state
        if (!playerReadyDictionary.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            playerReadyDictionary.Add(NetworkManager.Singleton.LocalClientId, false);
        }

        bool isReady = !playerReadyDictionary[NetworkManager.Singleton.LocalClientId];
        SetPlayerReadyServerRpc(isReady);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(bool isReady = false, ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = isReady;

        if (playerReadyDictionary.Count == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            bool allPlayersReady = true;
            foreach (var playerReady in playerReadyDictionary)
            {
                if (!playerReady.Value)
                {
                    allPlayersReady = false;
                    break;
                }
            }

            if (allPlayersReady)
            {
                // TODO - Map selection
                SceneLoaderWrapper.Instance.LoadScene(SceneType.Map_City_001.ToString(), GameDataSource.Instance.UseNetworkSceneManager());
            }
        }
    }

    public bool IsReady()
    {
        if (playerReadyDictionary.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            return playerReadyDictionary[NetworkManager.Singleton.LocalClientId];
        }
        return false;
    }
}
