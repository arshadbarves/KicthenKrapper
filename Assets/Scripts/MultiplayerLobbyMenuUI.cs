using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLobbyMenuUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private JoinCodeUI joinCodeUI;
    [SerializeField] private MessageUI messagePanel;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;


    private void Awake()
    {
        mainMenuButton.onClick.AddListener(MainMenuButton_OnClick);
        createLobbyButton.onClick.AddListener(CreateGameButton_OnClick);
        quickJoinButton.onClick.AddListener(QuickJoinButton_OnClick);
        joinCodeButton.onClick.AddListener(JoinCodeButton_OnClick);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void KitchenGameLobby_OnLobbyListChanged(object sender, KitchenGameLobby.LobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.Lobbies);
    }

    private void OnDestroy()
    {
        try
        {
            KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoinGame;
            KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
            KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        string message = NetworkManager.Singleton.DisconnectReason;

        if (string.IsNullOrEmpty(message))
        {
            message = "Failed to connect to game";
        }

        messagePanel.ShowMessage(message);
        quickJoinButton.interactable = true;

    }

    private void KitchenGameMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        messagePanel.ShowMessage("Trying to connect to game");
    }

    private void MainMenuButton_OnClick()
    {
        SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), false);
    }

    private void CreateGameButton_OnClick()
    {
        lobbyCreateUI.Show();
    }

    private void QuickJoinButton_OnClick()
    {
        quickJoinButton.interactable = false;
        KitchenGameMultiplayer.Instance.StartClient();
    }

    private void JoinCodeButton_OnClick()
    {
        joinCodeUI.Show();
    }

    private void UpdateLobbyList(List<Lobby> lobbies)
    {
        // Remove all the old lobbies except the template
        foreach (Transform child in lobbyContainer)
        {
            if (child != lobbyTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        // Add the new lobbies
        foreach (Lobby lobby in lobbies)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);

            LobbyListSingleUI lobbyUI = lobbyTransform.GetComponent<LobbyListSingleUI>();
            lobbyUI.SetLobby(lobby);
        }
    }
}

