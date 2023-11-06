using System;
using System.Collections.Generic;
using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public enum MatchType
    {
        Ranked,
        Casual
    }

    public class LobbyManager : NetworkSingleton<LobbyManager>
    {
        private const string LobbyLevel = "LEVEL";
        private const int MaxPlayers = 4;
        private const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
        private const int InviteIdLength = 10;

        public event Action<Lobby> LobbyCreated;
        public event Action LobbyCreatedFailed;
        public event Action<Lobby> LobbyJoined;
        public event Action LobbyJoinedFailed;
        public event Action LobbyLeft;
        public event Action<Lobby> LobbyUpdated;
        public event Action<string> LobbyResultsReceived;

        private MatchType currentMatchType;
        private Lobby currentLobbyCache;
        private bool lastCurrentLobbyIsValid = false;
        private EOSLobbyManager eOSLobbyManager;
        private NetworkList<PlayerData> lobbyPlayers = new NetworkList<PlayerData>();

        private void Start()
        {
            eOSLobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
            lobbyPlayers.OnListChanged += LobbyPlayers_OnListChanged;

            currentMatchType = MatchType.Ranked; // TODO: Remove this when we have a UI for selecting match type
        }

        private void LobbyPlayers_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            if (!IsServer)
                return;

            if (AllPlayersReady())
            {
                StartMatchmaking();
            }
        }

        private bool AllPlayersReady()
        {
            foreach (PlayerData playerReadyState in lobbyPlayers)
            {
                if (playerReadyState.playerGameState != PlayerGameState.Ready)
                {
                    return false;
                }
            }

            return true;
        }

        public override void OnDestroy()
        {
            eOSLobbyManager?.RemoveNotifyMemberUpdate(OnMemberUpdate);
            EOSManager.Instance.RemoveManager<EOSLobbyManager>();
        }

        private void Update()
        {
            HandleLobbyUpdate();
        }

        private void HandleLobbyUpdate()
        {
            Lobby currentLobby = eOSLobbyManager.GetCurrentLobby();

            if (currentLobby.IsValid())
            {
                if (currentLobbyCache != currentLobby)
                {
                    currentLobbyCache = currentLobby;
                    OnLobbyUpdated(Result.Success);
                    LobbyUpdated?.Invoke(currentLobby);
                }
            }

            if (currentLobby.IsValid() == lastCurrentLobbyIsValid)
            {
                return;
            }

            lastCurrentLobbyIsValid = currentLobby.IsValid();

            if (currentLobby.IsValid())
            {
                OnLobbyUpdated(Result.Success);
            }
            else
            {
                OnLeaveLobby(Result.Success);
            }
        }

        public Lobby GetCurrentLobby()
        {
            return eOSLobbyManager.GetCurrentLobby();
        }

        public void LeaveLobby()
        {
            eOSLobbyManager.LeaveLobby(OnLeaveLobby);
        }

        public void CreateLobby()
        {
            string uniqueInviteId = GenerateUniqueInviteId();

            if (!string.IsNullOrEmpty(uniqueInviteId))
            {
                Lobby lobbyProperties = new()
                {
                    BucketId = uniqueInviteId,
                    MaxNumLobbyMembers = MaxPlayers,
                    LobbyPermissionLevel = LobbyPermissionLevel.Publicadvertised,
                    AllowInvites = true,
                    PresenceEnabled = false, // TODO: Enable this when we have a presence system
                    RTCRoomEnabled = false
                };

                LobbyAttribute levelAttribute = new LobbyAttribute
                {
                    Key = LobbyLevel,
                    AsInt64 = GameManager.Instance.GetCurrentLevel().levelNumber,
                    ValueType = AttributeType.Int64,
                    Visibility = LobbyAttributeVisibility.Public
                };

                lobbyProperties.Attributes.Add(levelAttribute);
                eOSLobbyManager.CreateLobby(lobbyProperties, OnLobbyCreatedOrJoined);
            }
        }

        public void JoinLobby(Lobby lobbyRef, LobbyDetails lobbyDetailsRef)
        {
            eOSLobbyManager.JoinLobby(lobbyRef.Id, lobbyDetailsRef, true, OnLobbyCreatedOrJoined);
        }

        public void SearchLobbies(string inviteId)
        {
            eOSLobbyManager.SearchByAttribute("bucket", inviteId, OnSearchResultsReceived);
        }

        public void KickPlayer(ProductUserId productUserId)
        {
            eOSLobbyManager.KickMember(productUserId, null);
        }

        public void PromotePlayer(ProductUserId productUserId)
        {
            eOSLobbyManager.PromoteMember(productUserId, null);
        }

        public void AcceptInvite()
        {
            eOSLobbyManager.AcceptCurrentLobbyInvite(currentLobbyCache.AllowInvites, OnLobbyUpdated);
        }

        public void DeclineInvite()
        {
            eOSLobbyManager.DeclineLobbyInvite();
        }

        public void ModifyLobby()
        {
            Lobby currentLobby = eOSLobbyManager.GetCurrentLobby();

            if (currentLobby == null || !currentLobby.IsValid())
            {
                Debug.LogError("LobbyManager (ModifyLobby): CurrentLobby is invalid");
                return;
            }

            Lobby lobbyProperties = new()
            {
                BucketId = currentLobby.BucketId,
                MaxNumLobbyMembers = MaxPlayers,
                LobbyPermissionLevel = LobbyPermissionLevel.Publicadvertised,
                AllowInvites = true,
                PresenceEnabled = false, // TODO: Enable this when we have a presence system
                RTCRoomEnabled = false
            };

            LobbyAttribute levelAttribute = new LobbyAttribute
            {
                Key = LobbyLevel,
                AsInt64 = GameManager.Instance.GetCurrentLevel().levelNumber,
                ValueType = AttributeType.String,
                Visibility = LobbyAttributeVisibility.Public
            };

            lobbyProperties.Attributes.Add(levelAttribute);
            eOSLobbyManager.ModifyLobby(lobbyProperties, OnLobbyUpdated);
        }

        private void OnMemberUpdate(string LobbyId, ProductUserId MemberId)
        {
            Lobby currentLobby = eOSLobbyManager.GetCurrentLobby();
            if (currentLobby.Id != LobbyId)
            {
                return;
            }
        }

        private void OnLeaveLobby(Result result)
        {
            if (result != Result.Success)
            {
                Debug.LogErrorFormat("LobbyManager (OnLeaveLobby): LeaveLobby error '{0}'", result);
                return;
            }

            currentLobbyCache = null;
            lastCurrentLobbyIsValid = false;
            LobbyLeft?.Invoke();
        }

        private void OnLobbyCreatedOrJoined(Result result)
        {
            OnLobbyUpdated(result);
            Lobby currentLobby = eOSLobbyManager.GetCurrentLobby();

            if (currentLobby.IsOwner(EOSManager.Instance.GetProductUserId()))
            {
                LobbyCreated?.Invoke(currentLobby);
                MultiplayerManager.Instance.StartHost((bool success) =>
                {
                    if (success)
                    {
                        LobbyJoined?.Invoke(currentLobby);
                        lobbyPlayers.Add(GetLocalPlayer());
                    }
                });
            }
            else
            {
                if (currentLobby.LobbyOwner.IsValid())
                {
                    MultiplayerManager.Instance.StartClient(currentLobby.LobbyOwner, (bool success) =>
                    {
                        if (success)
                        {
                            LobbyJoined?.Invoke(currentLobby);
                            lobbyPlayers.Add(GetLocalPlayer());
                        }
                    });
                }
                else
                {
                    Debug.LogError("LobbyManager (OnLobbyUpdated): invalid server user id");
                }
            }
        }

        private void OnLobbyUpdated(Result result)
        {
            if (result != Result.Success)
            {
                LobbyCreatedFailed?.Invoke();
                Debug.LogErrorFormat("LobbyManager (OnLobbyUpdated): LobbyUpdate error '{0}'", result);
                return;
            }

            Lobby currentLobby = eOSLobbyManager.GetCurrentLobby();

            if (!currentLobby.IsValid())
            {
                LobbyCreatedFailed?.Invoke();
                Debug.LogErrorFormat("LobbyManager (OnLobbyUpdated): OnLobbyCreated returned invalid CurrentLobby.Id: {0}", currentLobby.Id);
                return;
            }
        }

        private void OnSearchResultsReceived(Result result)
        {
            if (result != Result.Success)
            {
                if (result == Result.NotFound || result == Result.InvalidParameters)
                {
                    Debug.Log("UILobbiesMenu (UpdateSearchResults): No results found.");
                    LobbyResultsReceived?.Invoke("No results found.");
                }
                else
                {
                    Debug.LogErrorFormat("UILobbiesMenu (UpdateSearchResults): result error '{0}'", result);
                    LobbyResultsReceived?.Invoke("Error: " + result);
                }
                return;
            }

            Dictionary<Lobby, LobbyDetails> lobbies = eOSLobbyManager.GetSearchResults();

            if (lobbies.Count == 0)
            {
                Debug.Log("UILobbiesMenu (UpdateSearchResults): No results found.");
                LobbyResultsReceived?.Invoke("No results found.");
                return;
            }

            if (lobbies.Count > 1)
            {
                Debug.LogWarningFormat("UILobbiesMenu (UpdateSearchResults): Found {0} lobbies, expected 1", lobbies.Count);
                LobbyResultsReceived?.Invoke("Found multiple lobbies. Joining the first one.");
            }

            foreach (KeyValuePair<Lobby, LobbyDetails> lobby in lobbies)
            {
                JoinLobby(lobby.Key, lobby.Value);
                break;
            }

            LobbyResultsReceived?.Invoke("Found a lobby");
        }

        public void StartMatchmaking()
        {
            MatchmakingManager.Instance.StartMatchmaking(currentMatchType);
        }

        public void CancelMatchmaking()
        {
            MatchmakingManager.Instance.CancelMatchmaking();
        }

        public void FindAndJoinLobby(string inviteId)
        {
            SearchLobbies(inviteId);
        }

        private string GenerateUniqueInviteId()
        {
            StringBuilder inviteIdBuilder = new StringBuilder();

            bool matchFound = false;

            while (!matchFound)
            {
                inviteIdBuilder.Clear();

                for (int i = 0; i < InviteIdLength; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(0, AllowedCharacters.Length);
                    char randomCharacter = AllowedCharacters[randomIndex];
                    inviteIdBuilder.Append(randomCharacter);
                }

                string inviteId = inviteIdBuilder.ToString().ToUpper();

                eOSLobbyManager.SearchByAttribute("bucket", inviteId, (Result result) =>
                {
                    if (result == Result.Success)
                    {
                        Debug.Log("Found a match, regenerating invite code");
                        matchFound = true;
                    }
                });
            }

            return inviteIdBuilder.ToString().ToUpper();
        }

        public PlayerData GetLocalPlayer()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;
            foreach (PlayerData player in lobbyPlayers)
            {
                if (player.clientId == localClientId)
                {
                    return player;
                }
            }

            PlayerData newPlayer = new PlayerData
            {
                clientId = NetworkManager.Singleton.LocalClientId,
                playerGameState = PlayerGameState.NotReady,
                playerName = GameManager.Instance.GetPlayerName(),
                playerId = EOSManager.Instance.GetProductUserId().ToString()
            };

            return newPlayer;
        }

        public List<PlayerData> GetPlayers()
        {
            List<PlayerData> players = new List<PlayerData>();
            foreach (PlayerData player in lobbyPlayers)
            {
                players.Add(player);
            }

            return players;
        }

        public void TogglePlayerReadyState()
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;
            for (int i = 0; i < lobbyPlayers.Count; i++)
            {
                if (lobbyPlayers[i].clientId == localClientId)
                {
                    PlayerData modifiedPlayer = lobbyPlayers[i];
                    modifiedPlayer.playerGameState = modifiedPlayer.playerGameState == PlayerGameState.NotReady ? PlayerGameState.Ready : PlayerGameState.NotReady;
                    lobbyPlayers[i] = modifiedPlayer;
                    break;
                }
            }
        }

        public void ReturnToLobby()
        {
            if (currentLobbyCache == null)
            {
                Debug.LogError("LobbyManager (ReturnToLobby): CurrentLobby is null");
                return;
            }

            lobbyPlayers.Clear();

            OnLobbyCreatedOrJoined(Result.Success);
        }

        public void SetMatchType(MatchType matchType)
        {
            currentMatchType = matchType;
        }

        public void UpdateLobbyPlayerList(string playerId, PlayerSessionState playerSessionState)
        {
            for (int i = 0; i < lobbyPlayers.Count; i++)
            {
                if (lobbyPlayers[i].playerId == playerId)
                {
                    PlayerData modifiedPlayer = lobbyPlayers[i];
                    modifiedPlayer.playerSessionState = playerSessionState;
                    lobbyPlayers[i] = modifiedPlayer;
                    break;
                }
            }
        }
    }
}
