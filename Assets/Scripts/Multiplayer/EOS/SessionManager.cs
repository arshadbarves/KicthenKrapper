using System;
using System.Collections.Generic;
using UnityEngine;
using Epic.OnlineServices;
using Epic.OnlineServices.Sessions;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace KitchenKrapper
{
    public class SessionManager : NetworkSingleton<SessionManager>
    {
        private const string SessionLevelKey = "LEVEL";
        private const int MaxPlayers = 8;

        private EOSSessionsManager sessionsManager;
        private Session currentSession;
        private SessionDetails currentSessionDetails;
        private bool isSessionReady;

        private void Start()
        {
            sessionsManager = EOSManager.Instance.GetOrCreateManager<EOSSessionsManager>();
        }

        public void Destroy()
        {
            EOSManager.Instance.RemoveManager<EOSSessionsManager>();
        }

        private void Update()
        {
            if (!IsServer || sessionsManager == null)
                return;

            bool stateUpdates = sessionsManager.Update();

            if (stateUpdates && currentSession != null && !isSessionReady)
            {
                if (currentSession.SessionState != OnlineSessionState.InProgress && currentSession.NumConnections == currentSession.MaxPlayers)
                {
                    Debug.Log("Session is full");
                    isSessionReady = true;
                }
            }
        }

        public void CreateSession(bool isPublic, bool isPresenceEnabled, Action callback = null)
        {
            Session session = CreateNewSession(isPublic);
            SessionAttribute attribute = CreateSessionAttribute();
            session.Attributes.Add(attribute);

            sessionsManager.CreateSession(session, isPresenceEnabled, () =>
            {
                currentSession = sessionsManager.GetCurrentSessions()[EOSManager.Instance.GetProductUserId().ToString()];
                callback?.Invoke();
            });
        }

        private Session CreateNewSession(bool isPublic)
        {
            return new Session
            {
                AllowJoinInProgress = false,
                InvitesAllowed = false,
                SanctionsEnabled = false,
                MaxPlayers = MaxPlayers,
                Name = EOSManager.Instance.GetProductUserId().ToString(),
                PermissionLevel = isPublic ? OnlineSessionPermissionLevel.PublicAdvertised : OnlineSessionPermissionLevel.InviteOnly
            };
        }

        private SessionAttribute CreateSessionAttribute()
        {
            return new SessionAttribute
            {
                Key = SessionLevelKey,
                AsInt64 = GameManager.Instance.GetCurrentLevel().levelNumber,
                ValueType = AttributeType.Int64,
                Advertisement = SessionAttributeAdvertisementType.Advertise
            };
        }

        public int GetSessionLevel()
        {
            if (currentSession == null)
            {
                Debug.LogError("No current session details found");
                return -1;
            }

            foreach (SessionAttribute attribute in currentSession.Attributes)
            {
                if (attribute.Key == SessionLevelKey)
                {
                    return (int)attribute.AsInt64;
                }
            }

            return -1;
        }

        public void JoinSession(Session session, SessionDetails sessionDetails, bool isPresenceEnabled, Action<Result> callback = null)
        {
            if (sessionDetails == null)
            {
                Debug.LogError("No joining session found");
                return;
            }

            sessionsManager.JoinSession(sessionDetails, isPresenceEnabled, (Result result) =>
            {
                if (result != Result.Success)
                {
                    Debug.LogError("Failed to join session");
                    return;
                }

                currentSession = session;
                currentSessionDetails = sessionDetails;
                callback?.Invoke(result);
            });
        }

        public void LeaveSession(Action<Result> callback = null)
        {
            void leaveSessionHandler(Result result)
            {
                if (result != Result.Success)
                {
                    Debug.LogError("Failed to leave session");
                    return;
                }

                currentSession = null;
                currentSessionDetails = null;
                callback?.Invoke(result);

                EOSSessionsManager.OnLeaveSession -= leaveSessionHandler;
            }

            EOSSessionsManager.OnLeaveSession += leaveSessionHandler;

            sessionsManager.DestroySession(currentSession.Name);
        }

        public void StartSession()
        {
            sessionsManager.StartSession(currentSession.Name);
        }

        public void EndSession(Action<Result> callback = null)
        {
            void endSessionHandler(Result result)
            {
                if (result != Result.Success)
                {
                    Debug.LogError("Failed to end session");
                    return;
                }

                callback?.Invoke(result);

                EOSSessionsManager.OnSessionEnded -= endSessionHandler;
            }

            EOSSessionsManager.OnSessionEnded += endSessionHandler;

            sessionsManager.EndSession(currentSession.Name);
        }

        public void SearchSessions(int searchPattern, Action<Dictionary<Session, SessionDetails>> callback = null)
        {
            SessionAttribute levelAttribute = CreateSearchSessionAttribute(searchPattern);

            void searchCompletedHandler()
            {
                callback?.Invoke(sessionsManager.GetCurrentSearch().GetResults());
                EOSSessionsManager.OnSessionSearchCompleted -= searchCompletedHandler;
            }

            EOSSessionsManager.OnSessionSearchCompleted += searchCompletedHandler;

            sessionsManager.Search(new List<SessionAttribute> { levelAttribute });
        }

        private SessionAttribute CreateSearchSessionAttribute(int searchPattern)
        {
            return new SessionAttribute
            {
                Key = SessionLevelKey,
                ValueType = AttributeType.Int64,
                AsInt64 = searchPattern,
                Advertisement = SessionAttributeAdvertisementType.Advertise
            };
        }

        public void SearchSessionById(string sessionId, Action<Dictionary<Session, SessionDetails>> callback = null)
        {
            void searchCompletedHandler()
            {
                callback?.Invoke(sessionsManager.GetCurrentSearch().GetResults());
                EOSSessionsManager.OnSessionSearchCompleted -= searchCompletedHandler;
            }

            EOSSessionsManager.OnSessionSearchCompleted += searchCompletedHandler;

            sessionsManager.SearchById(sessionId);
        }

        public Dictionary<Session, SessionDetails> GetSearchResults()
        {
            return sessionsManager.GetCurrentSearch().GetResults();
        }

        public Session GetCurrentSession()
        {
            return currentSession;
        }

        public SessionDetails GetCurrentSessionDetails()
        {
            return currentSessionDetails;
        }

        public Dictionary<string, Session> GetCurrentSessions()
        {
            return sessionsManager.GetCurrentSessions();
        }

        public void SetSessionReady(bool isReady)
        {
            isSessionReady = isReady;
        }

        public bool IsSessionReady()
        {
            return isSessionReady;
        }
    }
}
