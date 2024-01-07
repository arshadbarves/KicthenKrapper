using System;
using System.Collections.Generic;
using System.Linq;
using Epic.OnlineServices;
using Epic.OnlineServices.Sessions;
using Managers;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using UnityEngine;

namespace Multiplayer.EOS
{
    public class SessionManager : NetworkSingleton<SessionManager>
    {
        private const string SessionLevelKey = "LEVEL";
        private const int MaxPlayers = 1;
        private EOSSessionsManager _sessionsManager;
        private Session _currentSession;
        private SessionDetails _currentSessionDetails;
        private bool _isSessionFull;
        public Action OnSessionFull;

        private void Start()
        {
            _sessionsManager = EOSManager.Instance.GetOrCreateManager<EOSSessionsManager>();
        }

        private void OnApplicationQuit()
        {
            if (_currentSession == null) return;
            if (_currentSession.SessionState == OnlineSessionState.InProgress)
            {
                EndSession(result =>
                {
                    if (result != Result.Success)
                    {
                        Debug.Log("Failed to end the session");
                        return;
                    }
            
                    Debug.Log("Session ended");
                });
            }
            LeaveSession();
            while (_currentSession != null)
            {
            } // Wait for OnLeaveSession to be called
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            EOSManager.Instance.RemoveManager<EOSSessionsManager>();
        }

        private void Update()
        {
            if (!IsServer || _sessionsManager == null) return;
            var stateUpdates = _sessionsManager.Update();
            if (!stateUpdates || _currentSession == null) return;
            if (_currentSession.SessionState == OnlineSessionState.InProgress ||
                _currentSession.NumConnections != _currentSession.MaxPlayers || _isSessionFull)
                return;
            _isSessionFull = true;
            OnSessionFull.Invoke();
        }

        public void CreateSession(bool isPublic, bool isPresenceEnabled, Action callback = null)
        {
            var session = CreateNewSession(isPublic);
            var attribute = CreateSessionAttribute();
            session.Attributes.Add(attribute);
            _sessionsManager.CreateSession(session, isPresenceEnabled, () =>
            {
                _currentSession =
                    _sessionsManager.GetCurrentSessions()[EOSManager.Instance.GetProductUserId().ToString()];
                _isSessionFull = false;
                callback?.Invoke();
            });
        }

        private static Session CreateNewSession(bool isPublic)
        {
            return new Session
            {
                AllowJoinInProgress = false,
                InvitesAllowed = false,
                SanctionsEnabled = false,
                MaxPlayers = MaxPlayers,
                Name = EOSManager.Instance.GetProductUserId().ToString(),
                PermissionLevel =
                    isPublic
                        ? OnlineSessionPermissionLevel.PublicAdvertised
                        : OnlineSessionPermissionLevel.InviteOnly
            };
        }

        private static SessionAttribute CreateSessionAttribute()
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
            if (_currentSession == null)
            {
                Debug.LogError("No current session details found");
                return -1;
            }

            foreach (var attribute in _currentSession.Attributes.Where(attribute => attribute.Key == SessionLevelKey))
                if (attribute.AsInt64 != null)
                    return (int)attribute.AsInt64;
            return -1;
        }

        public void JoinSession(Session session, SessionDetails sessionDetails, bool isPresenceEnabled,
            Action<Result> callback = null)
        {
            if (sessionDetails == null)
            {
                Debug.LogError("No joining session found");
                return;
            }

            _sessionsManager.JoinSession(sessionDetails, isPresenceEnabled, result =>
            {
                if (result != Result.Success)
                {
                    Debug.LogError("Failed to join session");
                    return;
                }

                Debug.Log("Joined session" + session.Name);
                _currentSession = session;
                _currentSessionDetails = sessionDetails;
                callback?.Invoke(result);
            });
        }

        public void LeaveSession(Action<Result> callback = null)
        {
            EOSSessionsManager.OnLeaveSession += LeaveSessionHandler;
            _sessionsManager.DestroySession(_currentSession.Name);
            return;

            void LeaveSessionHandler(Result result)
            {
                if (result != Result.Success)
                {
                    Debug.LogError("Failed to leave session");
                    return;
                }

                _currentSession = null;
                _currentSessionDetails = null;
                _isSessionFull = false;
                callback?.Invoke(result);
                EOSSessionsManager.OnLeaveSession -= LeaveSessionHandler;
            }
        }

        public void StartSession()
        {
            _sessionsManager.StartSession(_currentSession.Name);
        }

        public void EndSession(Action<Result> callback = null)
        {
            EOSSessionsManager.OnSessionEnded += EndSessionHandler;
            _sessionsManager.EndSession(_currentSession.Name);
            return;

            void EndSessionHandler(Result result)
            {
                if (result != Result.Success)
                {
                    Debug.LogError("Failed to end session");
                    return;
                }

                callback?.Invoke(result);
                EOSSessionsManager.OnSessionEnded -= EndSessionHandler;
            }
        }

        public void SearchSessions(int searchPattern, Action<Dictionary<Session, SessionDetails>> callback = null)
        {
            var levelAttribute = CreateSearchSessionAttribute(searchPattern);
            EOSSessionsManager.OnSessionSearchCompleted += SearchCompletedHandler;
            _sessionsManager.Search(new List<SessionAttribute> { levelAttribute });
            return;

            void SearchCompletedHandler()
            {
                callback?.Invoke(_sessionsManager.GetCurrentSearch().GetResults());
                EOSSessionsManager.OnSessionSearchCompleted -= SearchCompletedHandler;
            }
        }

        private static SessionAttribute CreateSearchSessionAttribute(int searchPattern)
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
            EOSSessionsManager.OnSessionSearchCompleted += SearchCompletedHandler;
            _sessionsManager.SearchById(sessionId);
            return;

            void SearchCompletedHandler()
            {
                callback?.Invoke(_sessionsManager.GetCurrentSearch().GetResults());
                EOSSessionsManager.OnSessionSearchCompleted -= SearchCompletedHandler;
            }
        }

        public Dictionary<Session, SessionDetails> GetSearchResults()
        {
            return _sessionsManager.GetCurrentSearch().GetResults();
        }

        public Session GetCurrentSession()
        {
            return _currentSession;
        }

        public SessionDetails GetCurrentSessionDetails()
        {
            return _currentSessionDetails;
        }

        public Dictionary<string, Session> GetCurrentSessions()
        {
            return _sessionsManager.GetCurrentSessions();
        }
    }
}