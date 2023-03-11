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

        lobbyTemplate.gameObject.SetActive(false);
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
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        string message = NetworkManager.Singleton.DisconnectReason;

        if (string.IsNullOrEmpty(message))
        {
            message = "Failed to connect to game";
        }

        messagePanel.ShowMessage(message);

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
        KitchenGameMultiplayer.Instance.StartClient();
    }

    private void JoinCodeButton_OnClick()
    {
        joinCodeUI.Show();
    }

    private void UpdateLobbyList(List<Lobby> lobbies)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child.gameObject != lobbyTemplate)
                Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbies)
        {
            Transform lobbyObject = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyObject.gameObject.SetActive(true);
            lobbyObject.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}

