using System;
using System.Collections.Generic;
using UnityEngine;
using Epic.OnlineServices.Sessions;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using Epic.OnlineServices;
using Managers;
using Unity.Netcode;

namespace KitchenKrapper
{
    public class MatchmakingManager : NetworkSingleton<MatchmakingManager>
    {
        public static event Action MatchmakingStarted;
        public static event Action MatchmakingCanceled;
        public static event Action MatchmakingFailed;
        public static event Action MatchmakingSucceeded;

        public bool IsInQueue { get; private set; }

        private MatchType matchType;
        private Lobby currentLobby;

        private void Update()
        {
            if (!IsServer || !SessionManager.Instance.IsSessionReady())
                return;

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
            MatchmakingStarted?.Invoke();

            this.matchType = matchType;
            currentLobby = LobbyManager.Instance.GetCurrentLobby();

            Debug.Log("Starting matchmaking");

            if (currentLobby == null)
                FindBestMatchesOrCreateSession();
            else
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

            MatchmakingCanceled?.Invoke();
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

        private KeyValuePair<Session, SessionDetails>? FindBestSessionToJoin(Dictionary<Session, SessionDetails> sessions)
        {
            int playerCount = 1;
            foreach (KeyValuePair<Session, SessionDetails> kvp in sessions)
            {
                if (!ShouldIgnoreSession(kvp.Key, playerCount))
                {
                    return kvp;
                }
            }
            return null;
        }

        private bool ShouldIgnoreSession(Session session, int playerCount)
        {
            return session.NumConnections == session.MaxPlayers ||
                   session.NumConnections + playerCount > session.MaxPlayers ||
                   session.SessionState != OnlineSessionState.Pending;
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

            bool presenceEnabled = false;
            bool permissionLevel = matchType == MatchType.Ranked;

            SessionManager.Instance.CreateSession(permissionLevel, presenceEnabled, OnSessionCreated);
        }

        private void OnSessionCreated()
        {
            Debug.Log("Session created");
            MatchmakingSucceeded?.Invoke();
        }

        private void ExitGameSession()
        {
            Session currentSession = SessionManager.Instance.GetCurrentSession();
            if (currentSession == null)
            {
                Debug.Log("Not in a session");
                return;
            }

            SessionManager.Instance.EndSession((Result result) =>
            {
                if (result != Result.Success)
                {
                    Debug.Log("Failed to end the session");
                    return;
                }

                Debug.Log("Session ended");

                if (currentLobby != null)
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
            Debug.Log("Starting the match");
            SessionManager.Instance.StartSession();

            int levelNumber = SessionManager.Instance.GetSessionLevel() != -1
                ? SessionManager.Instance.GetSessionLevel()
                : GameManager.Instance.GetCurrentLevel().levelNumber;

            // TODO: Add a level number to level name mapping
            // SceneLoaderWrapper.Instance.LoadScene();
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
                Session joinedSession = SessionManager.Instance.GetCurrentSession();
                LobbyManager.Instance.UpdateLobbyPlayerList(EOSManager.Instance.GetProductUserId().ToString(), PlayerSessionState.Joined);

                ProductUserId hostId = ProductUserId.FromString(joinedSession.Name);
                if (LobbyManager.Instance.GetCurrentLobby() != null)
                {
                    // Do something when there's a lobby
                }
                else
                {
                    MultiplayerManager.Instance.StartClient(hostId);
                }

                MatchmakingSucceeded?.Invoke();
            }
            else
            {
                Debug.Log("Failed to join the session");
                LobbyManager.Instance.UpdateLobbyPlayerList(EOSManager.Instance.GetProductUserId().ToString(), PlayerSessionState.Failed);
                MatchmakingFailed?.Invoke();
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
