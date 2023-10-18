using System;
using UnityEngine;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using Epic.OnlineServices;
using Epic.OnlineServices.Sessions;
using System.Collections.Generic;
using Unity.Netcode;

namespace KitchenKrapper
{
    public enum PlayerSessionState
    {
        None,
        Joined,
        Left,
        Kicked,
        Banned,
        Disconnected,
        Failed
    }

    public class MatchmakingManager : MonoBehaviour
    {
        public const string SESSION_LEVEL = "LEVEL";
        private const int MAX_PLAYERS = 8;
        public static MatchmakingManager Instance { get; private set; }

        public static event Action OnMatchmakingStarted;
        public static event Action OnMatchmakingCanceled;
        public static event Action OnMatchmakingFailed;
        public static event Action OnMatchmakingSucceeded;


        public bool IsInQueue { get; private set; }
        private EOSSessionsManager sessionsManager;

        private bool isRankedMatch;
        private Lobby currentLobby;
        private NetworkVariable<Session> currentSession = new NetworkVariable<Session>();
        private NetworkVariable<Dictionary<string, PlayerSessionState>> lobbyPlayers = new NetworkVariable<Dictionary<string, PlayerSessionState>>(new Dictionary<string, PlayerSessionState>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private SessionDetails currentSessionDetails;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            sessionsManager = EOSManager.Instance.GetOrCreateManager<EOSSessionsManager>();
            currentSession.OnValueChanged += CurrentSession_OnValueChanged;
        }

        private void CurrentSession_OnValueChanged(Session previousValue, Session newValue)
        {
            SessionDetails newSessionDetails = sessionsManager.GetCurrentSearch().GetResults()[newValue];
            if (currentSessionDetails != newSessionDetails)
            {
                currentSessionDetails = newSessionDetails;
            }

            // lobbyPlayers
            foreach (KeyValuePair<string, PlayerSessionState> player in lobbyPlayers.Value)
            {
                if (player.Value == PlayerSessionState.Failed || player.Value == PlayerSessionState.Left)
                {
                    ExitAllLobbyPlayers();
                    return;
                }

                if (player.Value == PlayerSessionState.None)
                {
                    return;
                }
            }
        }

        private void ExitAllLobbyPlayers()
        {
            foreach (KeyValuePair<string, PlayerSessionState> player in lobbyPlayers.Value)
            {
                if (player.Value == PlayerSessionState.Joined)
                {
                    LeaveSessionServerRpc(player.Key);
                }
            }
        }

        private void Update()
        {
            if (currentSession != null && currentSession.Value != null)
            {
                if (currentSession.Value.SessionState == OnlineSessionState.Ended)
                {
                    Debug.Log("Session ended");
                    currentSession.Value = null;
                    lobbyPlayers.Value.Clear();
                    OnMatchmakingCanceled?.Invoke();
                }

                if (currentSession.Value.NumConnections == currentSession.Value.MaxPlayers)
                {
                    Debug.Log("Session starting");
                }
            }
        }

        private void OnDestroy()
        {
            // Unity crashes if you try to access EOSSinglton OnDestroy
            EOSManager.Instance.RemoveManager<EOSSessionsManager>();
        }

        public void StartMatchmaking(bool isRankedMatch, Lobby lobby = null)
        {
            if (IsInQueue)
            {
                Debug.Log("Already in queue");
                return;
            }

            IsInQueue = true;
            OnMatchmakingStarted?.Invoke();

            this.isRankedMatch = isRankedMatch;
            currentLobby = lobby;

            Debug.Log("Starting matchmaking");

            foreach (LobbyMember member in currentLobby.Members)
            {
                lobbyPlayers.Value.Add(member.ProductId.ToString(), PlayerSessionState.None);
            }

            currentSession.Value = FindBestMatch();

            if (currentSession != null)
            {
                Debug.Log("Found best match");

                currentSessionDetails = sessionsManager.GetCurrentSearch().GetResults()[currentSession.Value];
                if (currentLobby != null)
                {
                    JoinSessionServerRpc();
                }
                else
                {
                    JoinSession(currentSessionDetails);
                }
            }
            else
            {
                Debug.Log("No best match found");
                CreateSession();
            }
        }

        public void CancelMatchmaking()
        {
            if (!IsInQueue)
            {
                Debug.Log("Not in queue");
                return;
            }

            IsInQueue = false;

            OnMatchmakingCanceled?.Invoke();
            Debug.Log("Canceling matchmaking");
        }

        public Session FindBestMatch()
        {
            SearchSession(GameManager.Instance.GetCurrentLevel().levelNumber.ToString());
            Dictionary<Session, SessionDetails> sessions = sessionsManager.GetCurrentSearch().GetResults();
            Session bestMatch = null;


            foreach (KeyValuePair<Session, SessionDetails> kvp in sessions)
            {
                Session session = kvp.Key;
                SessionDetails details = kvp.Value;

                // Ignore sessions that are full or do not allow joining in progress
                if (session.NumConnections >= session.MaxPlayers || !session.AllowJoinInProgress || session.MaxPlayers - session.NumConnections < currentLobby.Members.Count || session.SessionState != OnlineSessionState.Starting)
                {
                    continue;
                }

                bestMatch = session;
                break;
            }

            return bestMatch;
        }

        public void SearchSession(string searchPattern)
        {
            if (string.IsNullOrEmpty(searchPattern))
            {
                return;
            }

            SessionAttribute levelAttribute = new SessionAttribute
            {
                Key = SESSION_LEVEL,
                ValueType = AttributeType.Int64,
                AsInt64 = long.Parse(searchPattern),
                Advertisement = SessionAttributeAdvertisementType.Advertise
            };

            List<SessionAttribute> attributes = new List<SessionAttribute>() { levelAttribute };

            sessionsManager.Search(attributes);
        }

        public void CreateSession()
        {
            if (!IsInQueue)
            {
                Debug.Log("Not in queue");
                return;
            }

            IsInQueue = false;
            Debug.Log("Creating session");


            bool joinViaLobby = currentLobby != null;

            bool presenceEnabled = false;

            Session session = new Session
            {
                AllowJoinInProgress = false,
                InvitesAllowed = false,
                SanctionsEnabled = false,
                MaxPlayers = MAX_PLAYERS,
                Name = "Test Session",
                PermissionLevel = isRankedMatch ? OnlineSessionPermissionLevel.PublicAdvertised : OnlineSessionPermissionLevel.InviteOnly
            };

            SessionAttribute attribute = new SessionAttribute
            {
                Key = SESSION_LEVEL,
                AsInt64 = joinViaLobby ? currentLobby.Attributes.Find((LobbyAttribute attrib) => { return attrib.Key == LobbyManager.LOBBY_LEVEL; }).AsInt64 : GameManager.Instance.GetCurrentLevel().levelNumber,
                ValueType = AttributeType.Int64,
                Advertisement = SessionAttributeAdvertisementType.Advertise
            };

            session.Attributes.Add(attribute);

            sessionsManager.CreateSession(session, presenceEnabled, OnSessionCreated);
        }

        private void OnSessionCreated()
        {
            Debug.Log("Session created");
            foreach (KeyValuePair<Session, SessionDetails> session in sessionsManager.GetCurrentSearch().GetResults())
            {
                Debug.Log("Session:" + session.Key.Name + " NumConnections:" + session.Key.NumConnections + " MaxPlayers:" + session.Key.MaxPlayers + " AllowJoinInProgress:" + session.Key.AllowJoinInProgress + " SessionState:" + session.Key.SessionState);
            }

            OnMatchmakingSucceeded?.Invoke();
        }

        public void LeaveSession()
        {
            if (currentSession == null)
            {
                Debug.Log("Not in session");
                return;
            }

            lobbyPlayers.Value[EOSManager.Instance.GetProductUserId().ToString()] = PlayerSessionState.Left;

            LeaveSessionServerRpc(EOSManager.Instance.GetProductUserId().ToString());
        }

        public void JoinSession(SessionDetails sessionDetails)
        {
            // Send Client Call to all lobby the players
            if (currentLobby != null)
            {
                sessionsManager.JoinSession(sessionDetails, false, OnSessionJoined);
            }
        }

        private void OnSessionJoined(Result result)
        {
            if (result == Result.Success)
            {
                Debug.Log("Session joined");
                lobbyPlayers.Value[EOSManager.Instance.GetProductUserId().ToString()] = PlayerSessionState.Joined;
                OnMatchmakingSucceeded?.Invoke();
            }
            else
            {
                Debug.Log("Failed to join session");
                lobbyPlayers.Value[EOSManager.Instance.GetProductUserId().ToString()] = PlayerSessionState.Failed;
                OnMatchmakingFailed?.Invoke();
            }
        }

        [ServerRpc]
        private void JoinSessionServerRpc()
        {
            if (currentSession == null)
            {
                Debug.Log("Not in session");
                return;
            }
            JoinSessionClientRpc();
        }

        [ClientRpc]
        private void JoinSessionClientRpc()
        {
            if (currentSession == null)
            {
                Debug.Log("Not in session");
                return;
            }
            sessionsManager.JoinSession(currentSessionDetails, false, OnSessionJoined);
        }

        [ServerRpc]
        private void LeaveSessionServerRpc(string player)
        {
            if (currentSession == null)
            {
                Debug.Log("Not in session");
                return;
            }
            LeaveSessionClientRpc(player);
        }

        [ClientRpc]
        private void LeaveSessionClientRpc(string player)
        {
            if (EOSManager.Instance.GetProductUserId().ToString() != player)
            {
                return;
            }

            if (currentSession == null)
            {
                Debug.Log("Not in session");
                return;
            }
            lobbyPlayers.Value[player] = PlayerSessionState.Left;
            sessionsManager.DestroySession(currentSession.Value.Id);
        }
    }
}