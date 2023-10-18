using System;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices.Samples;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class MultiplayerLobbyMenuUI : MonoBehaviour
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button createLobbyButton;
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
            joinCodeButton.onClick.AddListener(JoinCodeButton_OnClick);
        }

        private void Start()
        {
            EOSKitchenGameMultiplayer.Instance.OnTryingToJoinGame += EOSKitchenGameMultiplayer_OnTryingToJoinGame;
            EOSKitchenGameMultiplayer.Instance.OnFailedToJoinGame += EOSKitchenGameMultiplayer_OnFailedToJoinGame;
            // LobbyManager.Instance.OnLobbyListChanged += EOSKitchenGameLobby_OnLobbyListChanged;
            UpdateLobbyList(new Dictionary<Lobby, LobbyDetails>());
        }

        // private void EOSKitchenGameLobby_OnLobbyListChanged(object sender, LobbyManager.LobbyListChangedEventArgs e)
        // {
        //     UpdateLobbyList(e.Lobbies);
        // }

        private void OnDestroy()
        {
            try
            {
                EOSKitchenGameMultiplayer.Instance.OnTryingToJoinGame -= EOSKitchenGameMultiplayer_OnTryingToJoinGame;
                EOSKitchenGameMultiplayer.Instance.OnFailedToJoinGame -= EOSKitchenGameMultiplayer_OnFailedToJoinGame;
                // LobbyManager.Instance.OnLobbyListChanged -= EOSKitchenGameLobby_OnLobbyListChanged;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        private void EOSKitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
        {
            string message = NetworkManager.Singleton.DisconnectReason;

            if (string.IsNullOrEmpty(message))
            {
                message = "Failed to connect to game";
            }

            messagePanel.ShowMessage(message);

        }

        private void EOSKitchenGameMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
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

        private void JoinCodeButton_OnClick()
        {
            joinCodeUI.Show();
        }

        private void UpdateLobbyList(Dictionary<Lobby, LobbyDetails> lobbies)
        {
            // Remove all the old lobbies except the template
            foreach (Transform child in lobbyContainer)
            {
                if (child != lobbyTemplate)
                {
                    Destroy(child.gameObject);
                }
            }


            foreach (KeyValuePair<Lobby, LobbyDetails> kvp in lobbies)
            {
                if (kvp.Key == null)
                {
                    Debug.LogError("Lobbies (OnSearchResultsReceived): SearchResults has null key!");
                    continue;
                }

                if (kvp.Value == null)
                {
                    Debug.LogError("Lobbies (OnSearchResultsReceived): SearchResults has null Value!");
                    continue;
                }

                if (string.IsNullOrEmpty(kvp.Key.Id))
                {
                    Debug.LogWarning("Lobbies (OnSearchResultsReceived): Found lobby with null Id: ");
                    continue;
                }

                if (kvp.Key.LobbyOwner == null)
                {
                    Debug.LogWarningFormat("Lobbies (OnSearchResultsReceived): Found lobby with null LobbyOwner id: ", kvp.Key.Id);
                    continue;
                }

                Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
                lobbyTransform.gameObject.SetActive(true);

                LobbyListSingleUI lobbyUI = lobbyTransform.GetComponent<LobbyListSingleUI>();
                if (lobbyUI != null)
                {
                    /* TODO: Cache external/non-friend accounts
                    if (!kvp.Key.LobbyOwnerAccountId.IsValid())
                    {
                        kvp.Key.LobbyOwnerAccountId = FriendsManager.GetAccountMapping(kvp.Key.LobbyOwner);

                        if (!kvp.Key.LobbyOwnerAccountId.IsValid())
                        {
                            Debug.LogWarning("UILobbiesMenu (UIUpateSearchResults): LobbyOwner EpicAccountId not found in cache, need to query...");
                            // If still invalid, need to query for account information
                            // TODO query non cached
                        }
                    }

                    if (kvp.Key.LobbyOwnerAccountId.IsValid() && string.IsNullOrEmpty(kvp.Key.LobbyOwnerDisplayName))
                    {
                        lobbyUI.OwnerName = FriendsManager.GetDisplayName(kvp.Key.LobbyOwnerAccountId);

                        if (string.IsNullOrEmpty(kvp.Key.LobbyOwnerDisplayName))
                        {
                            Debug.LogWarning("UILobbiesMenu (Update): LobbyOwner DisplayName not found in cache, need to query...");
                            // No cached display name found for user, need to query for account information
                            // TODO query non cached
                        }
                    }
                    else
                    */
                    {
                        lobbyUI.OwnerName = kvp.Key.LobbyOwnerDisplayName;
                    }

                    // If DisplayName not found, display ProductUserId
                    if (string.IsNullOrEmpty(lobbyUI.OwnerName))
                    {
                        Result resultLobbyOwner = kvp.Key.LobbyOwner.ToString(out Utf8String outBuff);
                        if (resultLobbyOwner == Result.Success)
                        {
                            lobbyUI.OwnerName = outBuff;
                        }
                        else
                        {
                            lobbyUI.OwnerName = "Error: " + resultLobbyOwner;
                        }
                    }

                    lobbyUI.MaxMembers = (int)kvp.Key.MaxNumLobbyMembers;
                    lobbyUI.Members = (int)(lobbyUI.MaxMembers - kvp.Key.AvailableSlots);
                    lobbyUI.LobbyRef = kvp.Key;
                    lobbyUI.LobbyDetailsRef = kvp.Value;

                    // Get Level
                    var lobbyDetailsCopyAttributeByKeyOptions = new LobbyDetailsCopyAttributeByKeyOptions() { AttrKey = "LobbyManager.LOBBY_NAME" };
                    Result attrResult = kvp.Value.CopyAttributeByKey(ref lobbyDetailsCopyAttributeByKeyOptions, out Epic.OnlineServices.Lobby.Attribute? outAttrbite);
                    if (attrResult == Result.Success)
                    {
                        lobbyUI.LobbyName = outAttrbite?.Data?.Value.AsUtf8;
                    }
                    else
                    {
                        lobbyUI.LobbyName = "Error: " + attrResult;
                    }

                    lobbyUI.UpdateUI();
                }
            }

            // Add the new lobbies
            // foreach (Lobby lobby in lobbies)
            // {
            //     Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            //     lobbyTransform.gameObject.SetActive(true);

            //     LobbyListSingleUI lobbyUI = lobbyTransform.GetComponent<LobbyListSingleUI>();
            //     lobbyUI.SetLobby(lobby);
            // }
        }
    }
}