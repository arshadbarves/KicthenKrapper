using System;
using System.Collections.Generic;
using System.Linq;
using Epic.OnlineServices;
using Epic.OnlineServices.Sessions;
using KitchenKrapper;
using Managers;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using SceneManagement;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer.EOS
{
    public class MatchmakingManager : NetworkSingleton<MatchmakingManager>
    {
        public static event Action OnMatchmakingStarted;
        public static event Action OnMatchmakingCanceled;
        public static event Action OnMatchmakingFailed;
        public static event Action OnMatchmakingSucceeded;
        private bool IsInQueue { get; set; }
        private MatchType _matchType;
        private Lobby _currentLobby;

        private void Start()
        {
            SessionManager.Instance.OnSessionFull += OnSessionFullHandler;
        }
        
        public void OnDisable()
        {
            SessionManager.Instance.OnSessionFull -= OnSessionFullHandler;
        }

        private void OnSessionFullHandler()
        {
            Debug.Log("Session is full");
            StartMatch();
        }

        public void StartMatchmaking(MatchType matchType)
        {
            if (IsInQueue)
            {
                Debug.Log("Already in queue");
                return;
            }

            IsInQueue = true;
            OnMatchmakingStarted?.Invoke();
            _matchType = matchType;
            _currentLobby = LobbyManager.Instance.GetCurrentLobby();
            Debug.Log("Starting matchmaking");
            // if (!LobbyManager.Instance.IsCurrentLobbyValid())
            //     FindBestMatchesOrCreateSession();
            // else
                CreateSession();
        }

        public void CancelMatchmaking()
        {
            if (!IsInQueue)
            {
                Debug.Log("Not in queue");
                return;
            }

            IsInQueue = false;
            ExitGameSession();
            OnMatchmakingCanceled?.Invoke();
            Debug.Log("Canceling matchmaking");
        }

        private void FindBestMatchesOrCreateSession()
        {
            SessionManager.Instance.SearchSessions(GameManager.Instance.GetCurrentLevel().levelNumber, sessions =>
            {
                if (sessions.Count == 0)
                {
                    Debug.Log("No sessions found, creating a new session");
                    CreateSession();
                }
                else
                {
                    KeyValuePair<Session, SessionDetails>? session = FindBestSessionToJoin(sessions);
                    if (session == null)
                    {
                        Debug.Log("No suitable sessions found, creating a new session");
                        CreateSession();
                    }
                    else
                    {
                        Debug.Log("Found a session, joining it");
                        JoinSession(session.Value);
                    }
                }
            });
        }

        private static KeyValuePair<Session, SessionDetails>? FindBestSessionToJoin(
            Dictionary<Session, SessionDetails> sessions)
        {
            const int playerCount = 1;
            foreach (var kvp in sessions.Where(kvp => !ShouldIgnoreSession(kvp.Key, playerCount)))
            {
                return kvp;
            }

            return null;
        }

        private static bool ShouldIgnoreSession(Session session, int playerCount)
        {
            return session.NumConnections == session.MaxPlayers ||
                   session.NumConnections + playerCount > session.MaxPlayers ||
                   session.SessionState == OnlineSessionState.InProgress || session.NumConnections == session.MaxPlayers;
        }

        private void CreateSession()
        {
            if (!IsInQueue)
            {
                Debug.Log("Not in the queue");
                return;
            }

            IsInQueue = false;
            Debug.Log("Creating a session");
            const bool presenceEnabled = false;
            var permissionLevel = _matchType == MatchType.Ranked;
            SessionManager.Instance.CreateSession(permissionLevel, presenceEnabled, OnSessionCreated);
        }

        private void OnSessionCreated()
        {
            Debug.Log("Session created");
            MultiplayerManager.Instance.StartHost();
            OnMatchmakingSucceeded?.Invoke();
        }

        private void ExitGameSession()
        {
            var currentSession = SessionManager.Instance.GetCurrentSession();
            if (currentSession == null)
            {
                Debug.Log("Not in a session");
                return;
            }

            SessionManager.Instance.EndSession(result =>
            {
                if (result != Result.Success)
                {
                    Debug.Log("Failed to end the session");
                    return;
                }

                Debug.Log("Session ended");
                if (_currentLobby != null)
                {
                    ReturnToLobby();
                }
            });
        }

        private void ReturnToLobby()
        {
            LobbyManager.Instance.ReturnToLobby();
            LeaveSessionServerRpc();
        }

        private void StartMatch()
        {
            if(!IsServer) return;
            Debug.Log("Starting the match");
            SessionManager.Instance.StartSession();
            var levelNumber = SessionManager.Instance.GetSessionLevel() != -1
                ? SessionManager.Instance.GetSessionLevel()
                : GameManager.Instance.GetCurrentLevel().levelNumber;

            // TODO: Add a level number to level name mapping
            SceneLoaderWrapper.Instance.LoadScene(GameManager.Instance.GetCurrentLevel().sceneName, true);
        }

        private void JoinSession(KeyValuePair<Session, SessionDetails> session)
        {
            SessionManager.Instance.JoinSession(session.Key, session.Value, false, OnSessionJoined);
        }

        private void OnSessionJoined(Result result)
        {
            if (result == Result.Success)
            {
                Debug.Log("Session joined");
                var joinedSession = SessionManager.Instance.GetCurrentSession();
                LobbyManager.Instance.UpdateLobbyPlayerList(EOSManager.Instance.GetProductUserId().ToString(),
                    PlayerSessionState.Joined);
                var hostId = ProductUserId.FromString(joinedSession.Name);
                if (LobbyManager.Instance.GetCurrentLobby() != null)
                {
                    // Do something when there's a lobby
                }
                else
                {
                    MultiplayerManager.Instance.StartClient(hostId);
                }

                OnMatchmakingSucceeded?.Invoke();
            }
            else
            {
                Debug.Log("Failed to join the session");
                LobbyManager.Instance.UpdateLobbyPlayerList(EOSManager.Instance.GetProductUserId().ToString(),
                    PlayerSessionState.Failed);
                OnMatchmakingFailed?.Invoke();
            }
        }

        [ServerRpc]
        private void LeaveSessionServerRpc()
        {
            LeaveSessionClientRpc();
        }

        [ClientRpc]
        private void LeaveSessionClientRpc()
        {
            SessionManager.Instance.LeaveSession();
        }
    }
}