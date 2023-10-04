using System;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using UnityEngine;

namespace KitchenKrapper
{
    public class EOSKitchenGameLobby : MonoBehaviour
    {
        public const string BUCKET_ID = "LobbyBucket";
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

        private string lobbyIdValue;
        private string ownerIdValue;
        private ProductUserId currentLobbyOwnerCache;
        private bool lastCurrentLobbyIsValid = false;
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
            if (productUserId == null || !productUserId.IsValid() || !LobbyManager._Dirty)
            {
                return;
            }

            Lobby currentLobby = LobbyManager.GetCurrentLobby();

            if (currentLobby.IsValid())
            {
                bool ownerChanged = false;
                if (currentLobbyOwnerCache != currentLobby.LobbyOwner)
                {
                    ownerChanged = true;
                    Result resultLobbyOwner = currentLobby.LobbyOwner.ToString(out Utf8String outBuffer);
                    ownerIdValue = (resultLobbyOwner == Result.Success) ? outBuffer : "Error: " + resultLobbyOwner;
                    OnLobbyUpdated(Result.Success);
                    currentLobbyOwnerCache = currentLobby.LobbyOwner;
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
                    Debug.Log("UILobbiesMenu (UpdateSearchResults): No results found.");
                }
                else
                {
                    OnLobbyListChanged?.Invoke(this, new LobbyListChangedEventArgs() { Lobbies = new Dictionary<Lobby, LobbyDetails>() });
                    Debug.LogErrorFormat("UILobbiesMenu (UpdateSearchResults): result error '{0}'", result);
                }
                return;
            }

            OnLobbyListChanged?.Invoke(this, new LobbyListChangedEventArgs() { Lobbies = LobbyManager.GetSearchResults() });
        }

        private void OnDestroy()
        {
            LobbyManager?.RemoveNotifyMemberUpdate(OnMemberUpdate);
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
}
