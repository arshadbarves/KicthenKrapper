using UnityEngine;

namespace KitchenKrapper
{
    public class NetworkDisconnectedScreenController : MonoBehaviour
    {
        [SerializeField] private NetworkDisconnectedScreen networkDisconnectedScreen;

        private bool hasReconnectButtonShown = false;
        private float disconnectTime;
        public const float disconnectTimeout = 10.0f;

        private void OnEnable()
        {
            NetworkDisconnectedScreen.NetworkDisconnectedScreenShown += OnNetworkDisconnectedScreenShown;
            NetworkDisconnectedScreen.NetworkDisconnectedScreenHidden += OnNetworkDisconnectedScreenHidden;
        }

        private void OnDisable()
        {
            NetworkDisconnectedScreen.NetworkDisconnectedScreenShown -= OnNetworkDisconnectedScreenShown;
            NetworkDisconnectedScreen.NetworkDisconnectedScreenHidden -= OnNetworkDisconnectedScreenHidden;
        }

        private void OnNetworkDisconnectedScreenShown()
        {
            hasReconnectButtonShown = false;
            networkDisconnectedScreen.DisablePickable();
        }

        private void OnNetworkDisconnectedScreenHidden()
        {
            disconnectTime = 0.0f;
            networkDisconnectedScreen.HideReconnectButton();
        }

        private void Update()
        {
            if (!hasReconnectButtonShown)
            {
                disconnectTime += Time.deltaTime;
                if (disconnectTime >= disconnectTimeout)
                {
                    networkDisconnectedScreen.ShowReconnectButton();
                    networkDisconnectedScreen.EnablePickable();
                    hasReconnectButtonShown = true;
                }
            }
        }
    }
}