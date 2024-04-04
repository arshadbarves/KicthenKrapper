using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class NotifyUI : MonoBehaviour
    {
        [Header("Notify UI")]
        [SerializeField] private TextMeshProUGUI notifyText;
        [SerializeField] private MessageUI messagePanel;

        private void Start()
        {
            // LobbyManager.Instance.OnCreatedLobby += EOSKitchenGameLobby_OnCreatedLobby;
            // LobbyManager.Instance.OnCreatedLobbyFailed += EOSKitchenGameLobby_OnCreatedLobbyFailed;
            // LobbyManager.Instance.OnJoinedLobby += EOSKitchenGameLobby_OnJoinedLobby;
            // LobbyManager.Instance.OnJoinedLobbyFailed += EOSKitchenGameLobby_OnJoinedLobbyFailed;
            Hide();
        }

        private void OnDestroy()
        {
            // LobbyManager.Instance.OnCreatedLobby -= EOSKitchenGameLobby_OnCreatedLobby;
            // LobbyManager.Instance.OnCreatedLobbyFailed -= EOSKitchenGameLobby_OnCreatedLobbyFailed;
            // LobbyManager.Instance.OnJoinedLobby -= EOSKitchenGameLobby_OnJoinedLobby;
            // LobbyManager.Instance.OnJoinedLobbyFailed -= EOSKitchenGameLobby_OnJoinedLobbyFailed;
        }

        private void EOSKitchenGameLobby_OnJoinedLobbyFailed(object sender, EventArgs e)
        {
            if (NetworkManager.Singleton.DisconnectReason == "")
            {
                messagePanel.ShowMessage("Failed to join lobby");
            }
            else
            {
                messagePanel.ShowMessage(NetworkManager.Singleton.DisconnectReason);
            }

            Hide();
        }

        private void EOSKitchenGameLobby_OnJoinedLobby(object sender, EventArgs e)
        {
            ShowMessage("Joining Kitchen Lobby");
        }

        private void EOSKitchenGameLobby_OnCreatedLobbyFailed(object sender, EventArgs e)
        {
            if (NetworkManager.Singleton.DisconnectReason == "")
            {
                messagePanel.ShowMessage("Failed to create lobby");
            }
            else
            {
                messagePanel.ShowMessage(NetworkManager.Singleton.DisconnectReason);
            }

            Hide();
        }

        private void EOSKitchenGameLobby_OnCreatedLobby(object sender, EventArgs e)
        {
            ShowMessage("Creating Kitchen Lobby");
        }

        public void ShowMessage(string message)
        {
            notifyText.text = message;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}