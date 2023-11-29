using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
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

            if (LobbyManager.Instance != null)
            {
                Destroy(LobbyManager.Instance.gameObject);
            }
        }
    }
}