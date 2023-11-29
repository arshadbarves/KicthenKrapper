using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class DebuggerUI : MonoBehaviour
    {

        public void HostButtonClicked()
        {
            Debug.Log("Host button clicked");
            NetworkManager.Singleton.StartHost();
        }

        public void JoinButtonClicked()
        {
            Debug.Log("Join button clicked");
            NetworkManager.Singleton.StartClient();
        }
    }
}