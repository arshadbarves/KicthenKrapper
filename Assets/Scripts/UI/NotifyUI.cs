using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NotifyUI : MonoBehaviour
{
    [Header("Notify UI")]
    [SerializeField] private TextMeshProUGUI notifyText;
    [SerializeField] private MessageUI messagePanel;

    private void Start()
    {
        KitchenGameLobby.Instance.OnCreatedLobby += KitchenGameLobby_OnCreatedLobby;
        KitchenGameLobby.Instance.OnCreatedLobbyFailed += KitchenGameLobby_OnCreatedLobbyFailed;
        KitchenGameLobby.Instance.OnJoinedLobby += KitchenGameLobby_OnJoinedLobby;
        KitchenGameLobby.Instance.OnJoinedLobbyFailed += KitchenGameLobby_OnJoinedLobbyFailed;
        Hide();
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnCreatedLobby -= KitchenGameLobby_OnCreatedLobby;
        KitchenGameLobby.Instance.OnCreatedLobbyFailed -= KitchenGameLobby_OnCreatedLobbyFailed;
        KitchenGameLobby.Instance.OnJoinedLobby -= KitchenGameLobby_OnJoinedLobby;
        KitchenGameLobby.Instance.OnJoinedLobbyFailed -= KitchenGameLobby_OnJoinedLobbyFailed;
    }

    private void KitchenGameLobby_OnJoinedLobbyFailed(object sender, EventArgs e)
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

    private void KitchenGameLobby_OnJoinedLobby(object sender, EventArgs e)
    {
        ShowMessage("Joining Kitchen Lobby");
    }

    private void KitchenGameLobby_OnCreatedLobbyFailed(object sender, EventArgs e)
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

    private void KitchenGameLobby_OnCreatedLobby(object sender, EventArgs e)
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
