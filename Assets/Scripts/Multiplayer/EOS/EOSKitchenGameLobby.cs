using System;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using UnityEngine;

public class EOSKitchenGameLobby : MonoBehaviour
{

    // BUCKET_ID
    public const string BUCKET_ID = "LobbyBucket";
    // LOBBY_NAME
    public const string LOBBY_NAME = "LobbyName";

    public static EOSKitchenGameLobby Instance { get; private set; }

    public event EventHandler OnCreatedLobby;
    public event EventHandler OnCreatedLobbyFailed;
    public event EventHandler OnJoinedLobby;
    public event EventHandler OnJoinedLobbyFailed;

    public event EventHandler<LobbyListChangedEventArgs> OnLobbyListChanged;

    public class LobbyListChangedEventArgs : EventArgs
    {
        public Dictionary<Lobby, LobbyDetails> Lobbies;
    }

    // Current Lobby
    private string lobbyIdValue;
    private string ownerIdValue;

    // UI Cache
    private int lastMemberCount = 0;
    private ProductUserId currentLobbyOwnerCache;
    private bool lastCurrentLobbyIsValid = false;

    // Lobby Search
    private float listLobbyTimer = 0f;

    private EOSLobbyManager LobbyManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
    }

    private void Update()
    {
        HandleLobbyUpdate();
        HandlePeriodicLobbyUpdate();
    }

    private void HandlePeriodicLobbyUpdate()
    {
        // If we don't have a valid lobby, search for one periodically (every 3 seconds)
        if (lobbyIdValue == null || !LobbyManager.GetCurrentLobby().IsValid())
        {
            listLobbyTimer -= Time.deltaTime;
            if (listLobbyTimer <= 0f)
            {
                float listLobbyInterval = 3f;
                listLobbyTimer = listLobbyInterval;

                LobbyManager.SearchByAttribute("bucket", "LobbyBucket", OnSearchByAttribute);
            }
        }
    }

    private void HandleLobbyUpdate()
    {
        ProductUserId productUserId = EOSManager.Instance.GetProductUserId();
        if (productUserId == null || !productUserId.IsValid())
        {
            return;
        }

        if (!LobbyManager._Dirty)
        {
            return;
        }

        Lobby currentLobby = LobbyManager.GetCurrentLobby();

        if (currentLobby.IsValid())
        {
            bool ownerChanged = false;

            /* TODO: Cache external/non-friend accounts
            if(!currentLobby.LobbyOwnerAccountId.IsValid())
            {
                currentLobby.LobbyOwnerAccountId = FriendsManager.GetAccountMapping(currentLobby.LobbyOwner);

                if(!currentLobby.LobbyOwnerAccountId.IsValid())
                {
                    Debug.LogWarning("UILobbiesMenu (Update): LobbyOwner EpicAccountId not found in cache, need to query...");
                    // If still invalid, need to query for account information
                    // TODO query non cached
                }
            }

            if(currentLobby.LobbyOwnerAccountId.IsValid() && string.IsNullOrEmpty(currentLobby.LobbyOwnerDisplayName))
            {
                currentLobby.LobbyOwnerDisplayName = FriendsManager.GetDisplayName(currentLobby.LobbyOwnerAccountId);

                if(string.IsNullOrEmpty(currentLobby.LobbyOwnerDisplayName))
                {
                    Debug.LogWarning("UILobbiesMenu (Update): LobbyOwner DisplayName not found in cache, need to query...");
                    // No cached display name found for user, need to query for account information
                    // TODO query non cached
                }
            }
            */

            // Cache LobbyOwner
            if (currentLobbyOwnerCache != currentLobby.LobbyOwner)
            {
                ownerChanged = true;
                Result resultLobbyOwner = currentLobby.LobbyOwner.ToString(out Utf8String outBuffer);
                if (resultLobbyOwner == Result.Success)
                {
                    // Update owner
                    ownerIdValue = outBuffer;
                }
                else
                {
                    ownerIdValue = "Error: " + resultLobbyOwner;
                }

                OnLobbyUpdated(Result.Success);
                currentLobbyOwnerCache = currentLobby.LobbyOwner;
            }
        }

        // Invites UI Prompt
        // if (LobbyManager.GetCurrentInvite() != null)
        // {
        //     UIInvitePanel.SetActive(true);

        //     Result resultInviteFrom = LobbyManager.GetCurrentInvite().FriendId.ToString(out Utf8String outBuffer);
        //     if (resultInviteFrom == Result.Success)
        //     {
        //         // Update invite from
        //         InviteFromVal.text = outBuffer;
        //     }
        //     else
        //     {
        //         InviteFromVal.text = "Error: " + resultInviteFrom;
        //     }

        //     InviteLevelVal.text = LobbyManager.GetCurrentInvite().Lobby.Attributes[0].AsString;
        // }
        // else
        // {
        //     UIInvitePanel.SetActive(false);
        // }


        //Only When Valid Lobby changes
        if (currentLobby.IsValid() == lastCurrentLobbyIsValid)
        {
            return;
        }
        lastCurrentLobbyIsValid = currentLobby.IsValid();

        if (currentLobby.IsValid())
        {
            // Show Leave button and Update LobbyId UI
            OnLobbyUpdated(Result.Success);
        }
        else
        {
            // Clear UI
            OnLeaveLobby(Result.Success);
            lastMemberCount = 0;
            currentLobbyOwnerCache = null;
        }
    }

    public Lobby GetCurrentLobby()
    {
        return LobbyManager.GetCurrentLobby();
    }

    private void OnSearchByAttribute(Result result)
    {
        if (result != Result.Success)
        {
            if (result == Result.NotFound || result == Result.InvalidParameters)
            {
                OnLobbyListChanged?.Invoke(this, new LobbyListChangedEventArgs() { Lobbies = new Dictionary<Lobby, LobbyDetails>() });
                // It's not an error if there's no results found when searching or there's invalid characters in the search
                Debug.Log("UILobbiesMenu (UpdateSearchResults): No results found.");
            }
            else
            {
                OnLobbyListChanged?.Invoke(this, new LobbyListChangedEventArgs() { Lobbies = new Dictionary<Lobby, LobbyDetails>() });
                Debug.LogErrorFormat("UILobbiesMenu (UpdateSearchResults): result error '{0}'", result);
            }
            return;
        }

        // Update the lobby list
        OnLobbyListChanged?.Invoke(this, new LobbyListChangedEventArgs() { Lobbies = LobbyManager.GetSearchResults() });
    }

    private void OnDestroy()
    {
        LobbyManager?.RemoveNotifyMemberUpdate(OnMemberUpdate); // Unregister from LobbyManager

        EOSManager.Instance.RemoveManager<EOSLobbyManager>();
    }

    private void OnMemberUpdate(string LobbyId, ProductUserId MemberId)
    {
        Lobby currentLobby = LobbyManager.GetCurrentLobby();
        if (currentLobby.Id != LobbyId)
        {
            return;
        }
    }

    public void LeaveLobby()
    {
        LobbyManager.LeaveLobby(OnLeaveLobby);
    }

    private void OnLeaveLobby(Result result)
    {
        if (result != Result.Success)
        {
            Debug.LogErrorFormat("UILobbiesMenu (UIOnLeaveLobby): LeaveLobby error '{0}'", result);
            return;
        }

        lobbyIdValue = "";
        ownerIdValue = "";
        currentLobbyOwnerCache = null;
    }

    public void CreateLobby(Lobby lobbyProperties)
    {
        OnCreatedLobby?.Invoke(this, EventArgs.Empty);
        LobbyManager.CreateLobby(lobbyProperties, OnLobbyUpdated);
    }

    private void OnLobbyUpdated(Result result)
    {
        if (result != Result.Success)
        {
            OnCreatedLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.LogErrorFormat("UILobbiesMenu (UIOnLobbyUpdated): LobbyUpdate error '{0}'", result);
            return;
        }

        Lobby currentLobby = LobbyManager.GetCurrentLobby();

        if (!currentLobby.IsValid())
        {
            OnCreatedLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.LogErrorFormat("UILobbiesMenu (UIOnLobbyUpdated): OnLobbyCreated returned invalid CurrentLobby.Id: {0}", currentLobby.Id);
            return;
        }

        string lobbyId = currentLobby.Id;

        if (!string.IsNullOrEmpty(lobbyId))
        {
            lobbyIdValue = currentLobby.Id;
        }

        if (currentLobby.IsOwner(EOSManager.Instance.GetProductUserId()))
        {
            Debug.Log("UIOnLobbyUpdated (UIOnLobbyUpdated): Joined as Host (enable ModifyLobby button)");
            EOSKitchenGameMultiplayer.Instance.StartHost();
        }
        else
        {
            Debug.Log("UIOnLobbyUpdated (UIOnLobbyUpdated): Joined as Client (disable ModifyLobby button)");

            if (currentLobby.LobbyOwner.IsValid())
            {
                EOSKitchenGameMultiplayer.Instance.StartClient(currentLobby.LobbyOwner);
            }
            else
            {
                Debug.LogError("UIP2PTransportMenu (JoinGame): invalid server user id");
            }
        }
    }

    public void JoinLobby(Lobby lobbyRef, LobbyDetails lobbyDetailsRef)
    {
        OnJoinedLobby?.Invoke(this, EventArgs.Empty);
        LobbyManager.JoinLobby(lobbyRef.Id, lobbyDetailsRef, true, OnLobbyUpdated);
    }
}