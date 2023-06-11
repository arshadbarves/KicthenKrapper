using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class KitchenGameLobby : MonoBehaviour
{
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public static KitchenGameLobby Instance { get; private set; }

    public event EventHandler OnCreatedLobby;
    public event EventHandler OnCreatedLobbyFailed;
    public event EventHandler OnJoinedLobby;
    public event EventHandler OnJoinedLobbyFailed;
    public event EventHandler<LobbyListChangedEventArgs> OnLobbyListChanged;

    public class LobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> Lobbies;
    }

    private Lobby joinedLobby;
    private float heartBeatTimer = 0f;
    private float listLobbyTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile("Profile" + UnityEngine.Random.Range(0, 1000000).ToString());
            await UnityServices.InitializeAsync(options);
            // await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
        HandleHeartBeat();
        HandlePeriodicLobbyUpdate();
    }

    private void HandlePeriodicLobbyUpdate()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneLoaderWrapper.Instance.GetActiveSceneName() == SceneType.MultiplayerMenu.ToString())
        {
            listLobbyTimer -= Time.deltaTime;
            if (listLobbyTimer <= 0f)
            {
                float listLobbyInterval = 5f;
                listLobbyTimer = listLobbyInterval;
                GetLobbies();
            }
        }
    }

    private void HandleHeartBeat()
    {
        if (joinedLobby == null)
            return;
        if (!IsLobbyOwner())
            return;
        if (heartBeatTimer <= 0f)
        {
            float heartBeatInterval = 15f;
            heartBeatTimer = heartBeatInterval;

            LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
    }

    private bool IsLobbyOwner()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void GetLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>{
                new QueryFilter
                (
                    QueryFilter.FieldOptions.AvailableSlots,
                    "0",
                    QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);

            OnLobbyListChanged?.Invoke(this, new LobbyListChangedEventArgs { Lobbies = queryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }

    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYERS - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);

            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string relayJoinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);

            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreatedLobby?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYERS, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData
            (
                allocation,
                "dtls"
            ));

            KitchenGameMultiplayer.Instance.StartHost();
            SceneLoaderWrapper.Instance.LoadScene(SceneType.Lobby.ToString(), true);
        }
        catch (LobbyServiceException e)
        {
            OnCreatedLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.LogError(e.Message);
        }
    }

    public async void QuickJoinLobby()
    {
        OnJoinedLobby?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData
            (
                joinAllocation,
                "dtls"
            ));

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
            OnJoinedLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public Lobby GetLobby()
    {
        if (joinedLobby != null)
        {
            return joinedLobby;
        }

        return null;
    }

    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinedLobby?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData
            (
                joinAllocation,
                "dtls"
            ));


            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
            OnJoinedLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public async void LeaveLobby()
    {
        // If the player is the host, delete the lobby and stop the host connection
        if (joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            DeleteLobby();
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public async void JoinWithId(string lobbyId)
    {
        OnJoinedLobby?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData
            (
                joinAllocation,
                "dtls"
            ));


            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
            OnJoinedLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }
}
