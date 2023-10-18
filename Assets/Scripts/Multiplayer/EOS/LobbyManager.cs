using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using UnityEngine;

namespace KitchenKrapper
{
    public class LobbyManager : MonoBehaviour
    {
        public const string LOBBY_LEVEL = "LEVEL";
        private const int MAX_PLAYERS = 4;
        private const string allowedCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

        public event Action<Lobby> LobbyCreated;
        public event Action LobbyCreatedFailed;
        public event Action<Lobby> LobbyJoined;
        public event Action LobbyJoinedFailed;
        public event Action LobbyLeft;
        public event Action<Lobby> LobbyUpdated;
        public event Action<string> LobbyResultsReceived;

        public static LobbyManager Instance { get; private set; }

        private bool isRankedMatch = true; // TODO: Make a friend match game mode system
        private Lobby currentLobbyCache;
        private bool lastCurrentLobbyIsValid = false;
        private EOSLobbyManager eOSLobbyManager;
        private MatchmakingManager matchmakingManager;

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
            eOSLobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
            matchmakingManager = MatchmakingManager.Instance;
        }

        private void OnDestroy()
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
            ProductUserId productUserId = EOSManager.Instance.GetProductUserId();
            if (productUserId == null || !productUserId.IsValid() || !eOSLobbyManager._Dirty)
            {
                return;
            }

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
                currentLobbyCache = null;
                LobbyLeft?.Invoke();
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
            StartCoroutine(CreateUniqueInviteIdCoroutine((string inviteId) =>
            {
                Lobby lobbyProperties = new()
                {
                    BucketId = inviteId,
                    MaxNumLobbyMembers = MAX_PLAYERS,
                    LobbyPermissionLevel = LobbyPermissionLevel.Publicadvertised,
                    AllowInvites = true,
                    PresenceEnabled = false, // TODO: Enable this when we have a presence system
                    RTCRoomEnabled = false
                };

                LobbyAttribute levelAttribute = new LobbyAttribute
                {
                    Key = LOBBY_LEVEL,
                    AsInt64 = GameManager.Instance.GetCurrentLevel().levelNumber,
                    ValueType = AttributeType.Int64,
                    Visibility = LobbyAttributeVisibility.Public
                };

                lobbyProperties.Attributes.Add(levelAttribute);
                eOSLobbyManager.CreateLobby(lobbyProperties, OnLobbyUpdated);
            }));
        }

        public void JoinLobby(Lobby lobbyRef, LobbyDetails lobbyDetailsRef)
        {
            eOSLobbyManager.JoinLobby(lobbyRef.Id, lobbyDetailsRef, true, OnLobbyUpdated);
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
                MaxNumLobbyMembers = MAX_PLAYERS,
                LobbyPermissionLevel = LobbyPermissionLevel.Publicadvertised,
                AllowInvites = true,
                PresenceEnabled = false, // TODO: Enable this when we have a presence system
                RTCRoomEnabled = false
            };

            LobbyAttribute levelAttribute = new LobbyAttribute
            {
                Key = LOBBY_LEVEL,
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

        private IEnumerator CreateUniqueInviteIdCoroutine(Action<string> onInviteIdGenerated)
        {
            int inviteIdLength = 10;
            StringBuilder inviteIdBuilder = new StringBuilder();

            for (int i = 0; i < inviteIdLength; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, allowedCharacters.Length);
                char randomCharacter = allowedCharacters[randomIndex];
                inviteIdBuilder.Append(randomCharacter);
            }

            string inviteId = inviteIdBuilder.ToString().ToUpper();

            bool matchFound = false;

            eOSLobbyManager.SearchByAttribute("bucket", inviteId, (Result result) =>
            {
                if (result == Result.Success)
                {
                    Debug.Log("Found a match, regenerating invite code");
                    matchFound = true;
                }
            });

            yield return new WaitUntil(() => !matchFound);

            onInviteIdGenerated?.Invoke(inviteId);
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

            if (currentLobby.IsOwner(EOSManager.Instance.GetProductUserId()))
            {
                LobbyCreated?.Invoke(currentLobby);
            }
            else
            {
                LobbyJoined?.Invoke(currentLobby);
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
            matchmakingManager.StartMatchmaking(isRankedMatch, GetCurrentLobby());
        }
    }
}
