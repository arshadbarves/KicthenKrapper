using System;
using System.Collections.Generic;
using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using KitchenKrapper;
using Managers;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer.EOS
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

        public event Action<Lobby> OnLobbyCreated;
        public event Action OnLobbyCreatedFailed;
        public event Action<Lobby> OnLobbyJoined;
        public event Action OnLobbyJoinedFailed;
        public event Action OnLobbyLeft;
        public event Action<Lobby> OnLobbyUpdated;
        public event Action<string> OnLobbyResultsReceived;

        private MatchType _currentMatchType;
        private Lobby _currentLobbyCache;
        private bool _lastCurrentLobbyIsValid;
        private EOSLobbyManager _eOSLobbyManager;
        private readonly NetworkList<PlayerData> _lobbyPlayers = new();

        private void Start()
        {
            _eOSLobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
            _lobbyPlayers.OnListChanged += LobbyPlayers_OnListChanged;

            _currentMatchType = MatchType.Ranked; // TODO: Remove this when we have a UI for selecting match type
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
            foreach (var playerReadyState in _lobbyPlayers)
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
            _eOSLobbyManager?.RemoveNotifyMemberUpdate(OnMemberUpdate);
            EOSManager.Instance.RemoveManager<EOSLobbyManager>();
        }

        private void Update()
        {
            HandleLobbyUpdate();
        }

        private void HandleLobbyUpdate()
        {
            var currentLobby = _eOSLobbyManager.GetCurrentLobby();

            if (currentLobby.IsValid())
            {
                if (_currentLobbyCache != currentLobby)
                {
                    _currentLobbyCache = currentLobby;
                    OnLobbyUpdatedHandler(Result.Success);
                    OnLobbyUpdated?.Invoke(currentLobby);
                }
            }

            if (currentLobby.IsValid() == _lastCurrentLobbyIsValid)
            {
                return;
            }

            _lastCurrentLobbyIsValid = currentLobby.IsValid();

            if (currentLobby.IsValid())
            {
                OnLobbyUpdatedHandler(Result.Success);
            }
            else
            {
                OnLeaveLobby(Result.Success);
            }
        }

        public Lobby GetCurrentLobby()
        {
            return _eOSLobbyManager.GetCurrentLobby();
        }
        
        public bool IsCurrentLobbyValid()
        {
            return _eOSLobbyManager.GetCurrentLobby().IsValid();
        }

        public void LeaveLobby()
        {
            _eOSLobbyManager.LeaveLobby(OnLeaveLobby);
        }

        public void CreateLobby()
        {
            var uniqueInviteId = GenerateUniqueInviteId();

            if (string.IsNullOrEmpty(uniqueInviteId)) return;
            Lobby lobbyProperties = new()
            {
                BucketId = uniqueInviteId,
                MaxNumLobbyMembers = MaxPlayers,
                LobbyPermissionLevel = LobbyPermissionLevel.Publicadvertised,
                AllowInvites = true,
                PresenceEnabled = false, // TODO: Enable this when we have a presence system
                RTCRoomEnabled = false
            };

            var levelAttribute = new LobbyAttribute
            {
                Key = LobbyLevel,
                AsInt64 = GameManager.Instance.GetCurrentLevel().levelNumber,
                ValueType = AttributeType.Int64,
                Visibility = LobbyAttributeVisibility.Public
            };

            lobbyProperties.Attributes.Add(levelAttribute);
            _eOSLobbyManager.CreateLobby(lobbyProperties, OnLobbyCreatedOrJoined);
        }

        public void JoinLobby(Lobby lobbyRef, LobbyDetails lobbyDetailsRef)
        {
            _eOSLobbyManager.JoinLobby(lobbyRef.Id, lobbyDetailsRef, true, OnLobbyCreatedOrJoined);
        }

        private void SearchLobbies(string inviteId)
        {
            _eOSLobbyManager.SearchByAttribute("bucket", inviteId, OnSearchResultsReceived);
        }

        public void KickPlayer(ProductUserId productUserId)
        {
            _eOSLobbyManager.KickMember(productUserId, null);
        }

        public void PromotePlayer(ProductUserId productUserId)
        {
            _eOSLobbyManager.PromoteMember(productUserId, null);
        }

        public void AcceptInvite()
        {
            _eOSLobbyManager.AcceptCurrentLobbyInvite(_currentLobbyCache.AllowInvites, OnLobbyUpdatedHandler);
        }

        public void DeclineInvite()
        {
            _eOSLobbyManager.DeclineLobbyInvite();
        }

        public void ModifyLobby()
        {
            var currentLobby = _eOSLobbyManager.GetCurrentLobby();

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

            var levelAttribute = new LobbyAttribute
            {
                Key = LobbyLevel,
                AsInt64 = GameManager.Instance.GetCurrentLevel().levelNumber,
                ValueType = AttributeType.String,
                Visibility = LobbyAttributeVisibility.Public
            };

            lobbyProperties.Attributes.Add(levelAttribute);
            _eOSLobbyManager.ModifyLobby(lobbyProperties, OnLobbyUpdatedHandler);
        }

        private void OnMemberUpdate(string lobbyId, ProductUserId memberId)
        {
            var currentLobby = _eOSLobbyManager.GetCurrentLobby();
            if (currentLobby.Id != lobbyId)
            {
                // This is not the lobby we are looking for
            }
        }

        private void OnLeaveLobby(Result result)
        {
            if (result != Result.Success)
            {
                Debug.LogErrorFormat("LobbyManager (OnLeaveLobby): LeaveLobby error '{0}'", result);
                return;
            }

            _currentLobbyCache = null;
            _lastCurrentLobbyIsValid = false;
            OnLobbyLeft?.Invoke();
        }

        private void OnLobbyCreatedOrJoined(Result result)
        {
            OnLobbyUpdatedHandler(result);
            var currentLobby = _eOSLobbyManager.GetCurrentLobby();

            if (currentLobby.IsOwner(EOSManager.Instance.GetProductUserId()))
            {
                OnLobbyCreated?.Invoke(currentLobby);
                MultiplayerManager.Instance.StartHost(success =>
                {
                    if (!success) return;
                    OnLobbyJoined?.Invoke(currentLobby);
                    _lobbyPlayers.Add(GetLocalPlayer());
                });
            }
            else
            {
                if (currentLobby.LobbyOwner.IsValid())
                {
                    MultiplayerManager.Instance.StartClient(currentLobby.LobbyOwner, success =>
                    {
                        if (!success) return;
                        OnLobbyJoined?.Invoke(currentLobby);
                        _lobbyPlayers.Add(GetLocalPlayer());
                    });
                }
                else
                {
                    Debug.LogError("LobbyManager (OnLobbyUpdatedHandler): invalid server user id");
                }
            }
        }

        private void OnLobbyUpdatedHandler(Result result)
        {
            if (result != Result.Success)
            {
                OnLobbyCreatedFailed?.Invoke();
                Debug.LogErrorFormat("LobbyManager (OnLobbyUpdatedHandler): LobbyUpdate error '{0}'", result);
                return;
            }

            var currentLobby = _eOSLobbyManager.GetCurrentLobby();

            if (currentLobby.IsValid()) return;
            OnLobbyCreatedFailed?.Invoke();
            Debug.LogErrorFormat("LobbyManager (OnLobbyUpdatedHandler): OnLobbyCreated returned invalid CurrentLobby.Id: {0}", currentLobby.Id);
        }

        private void OnSearchResultsReceived(Result result)
        {
            if (result != Result.Success)
            {
                if (result == Result.NotFound || result == Result.InvalidParameters)
                {
                    Debug.Log("UILobbiesMenu (UpdateSearchResults): No results found.");
                    OnLobbyResultsReceived?.Invoke("No results found.");
                }
                else
                {
                    Debug.LogErrorFormat("UILobbiesMenu (UpdateSearchResults): result error '{0}'", result);
                    OnLobbyResultsReceived?.Invoke("Error: " + result);
                }
                return;
            }

            var lobbies = _eOSLobbyManager.GetSearchResults();

            switch (lobbies.Count)
            {
                case 0:
                    Debug.Log("UILobbiesMenu (UpdateSearchResults): No results found.");
                    OnLobbyResultsReceived?.Invoke("No results found.");
                    return;
                case > 1:
                    Debug.LogWarningFormat("UILobbiesMenu (UpdateSearchResults): Found {0} lobbies, expected 1", lobbies.Count);
                    OnLobbyResultsReceived?.Invoke("Found multiple lobbies. Joining the first one.");
                    break;
            }

            foreach (var lobby in lobbies)
            {
                JoinLobby(lobby.Key, lobby.Value);
                break;
            }

            OnLobbyResultsReceived?.Invoke("Found a lobby");
        }

        public void StartMatchmaking()
        {
            MatchmakingManager.Instance.StartMatchmaking(_currentMatchType);
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
            var inviteIdBuilder = new StringBuilder();

            var matchFound = false;

            while (!matchFound)
            {
                inviteIdBuilder.Clear();

                for (var i = 0; i < InviteIdLength; i++)
                {
                    var randomIndex = UnityEngine.Random.Range(0, AllowedCharacters.Length);
                    var randomCharacter = AllowedCharacters[randomIndex];
                    inviteIdBuilder.Append(randomCharacter);
                }

                var inviteId = inviteIdBuilder.ToString().ToUpper();

                _eOSLobbyManager.SearchByAttribute("bucket", inviteId, result =>
                {
                    if (result != Result.Success) return;
                    Debug.Log("Found a match, regenerating invite code");
                    matchFound = true;
                });
            }

            return inviteIdBuilder.ToString().ToUpper();
        }

        private PlayerData GetLocalPlayer()
        {
            var localClientId = NetworkManager.Singleton.LocalClientId;
            foreach (var player in _lobbyPlayers)
            {
                if (player.clientId == localClientId)
                {
                    return player;
                }
            }

            var newPlayer = new PlayerData
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
            var players = new List<PlayerData>();
            foreach (var player in _lobbyPlayers)
            {
                players.Add(player);
            }

            return players;
        }

        public void TogglePlayerReadyState()
        {
            var localClientId = NetworkManager.Singleton.LocalClientId;
            for (var i = 0; i < _lobbyPlayers.Count; i++)
            {
                if (_lobbyPlayers[i].clientId != localClientId) continue;
                var modifiedPlayer = _lobbyPlayers[i];
                modifiedPlayer.playerGameState = modifiedPlayer.playerGameState == PlayerGameState.NotReady ? PlayerGameState.Ready : PlayerGameState.NotReady;
                _lobbyPlayers[i] = modifiedPlayer;
                break;
            }
        }

        public void ReturnToLobby()
        {
            if (_currentLobbyCache == null)
            {
                Debug.LogError("LobbyManager (ReturnToLobby): CurrentLobby is null");
                return;
            }

            _lobbyPlayers.Clear();

            OnLobbyCreatedOrJoined(Result.Success);
        }

        public void SetMatchType(MatchType matchType)
        {
            _currentMatchType = matchType;
        }

        public void UpdateLobbyPlayerList(string playerId, PlayerSessionState playerSessionState)
        {
            for (var i = 0; i < _lobbyPlayers.Count; i++)
            {
                if (_lobbyPlayers[i].playerId != playerId) continue;
                var modifiedPlayer = _lobbyPlayers[i];
                modifiedPlayer.playerSessionState = playerSessionState;
                _lobbyPlayers[i] = modifiedPlayer;
                break;
            }
        }
    }
}
