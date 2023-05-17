using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (EOSKitchenGameMultiplayer.Instance != null)
        {
            Destroy(EOSKitchenGameMultiplayer.Instance.gameObject);
        }

        if (EOSKitchenGameLobby.Instance != null)
        {
            Destroy(EOSKitchenGameLobby.Instance.gameObject);
        }
    }
}
