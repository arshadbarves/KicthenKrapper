#if NAKAMA_MULTIPLAYER
using System;
using System.Threading.Tasks;
using Nakama;
using RecipeRage.Managers;
using RecipeRage.Multiplayer.NakamaServer;
using RecipeRage.Utilities;
using UnityEngine;

namespace RecipeRage.NakamaServer
{
    public class NakamaManager : Singleton<NakamaManager>
    {
        private NakamaConnectionData _nakamaConnectionData;


        private IClient _connectionClient;
        private ISession _userSession;
        private ISocket _connectionSocket;


        private string _currentMatchmakingTicket;
        private string _currentMatchId;

        public IApiAccount CurrentAccount { get; private set; }

        public void Initialize(NakamaConnectionData nakamaConnection)
        {
            _nakamaConnectionData = nakamaConnection;
        }

        public async Task ConnectToServer()
        {
            _connectionClient = new Client(_nakamaConnectionData.Scheme, _nakamaConnectionData.Host,
                _nakamaConnectionData.Port, _nakamaConnectionData.ServerKey, UnityWebRequestAdapter.Instance);
            _connectionClient.Timeout = 5; // 5 seconds

            _connectionSocket = _connectionClient.NewSocket(useMainThread: true);

            string authToken = PlayerPrefs.GetString(GameConstants.AuthTokenKey, null);
            bool isAuthToken = !string.IsNullOrEmpty(authToken);

            string refreshToken = PlayerPrefs.GetString(GameConstants.RefreshTokenKey, null);

            if (isAuthToken)
            {
                _userSession = Session.Restore(authToken, refreshToken);
                // Check whether a session is close to expiry. e.g. 1 day before expiry. So we can refresh the session.
                if (_userSession.HasExpired(DateTime.UtcNow.AddDays(1)))
                {
                    try
                    {
                        // get a new access token
                        _userSession = await _connectionClient.SessionRefreshAsync(_userSession);
                    }
                    catch (ApiResponseException)
                    {
                        UIManager.Instance.ShowLoginScreen();
                        return;
                    }
                }
            }
            else
            {
                UIManager.Instance.ShowLoginScreen();
                return;
            }

            await HandlePostLoginActions();
        }

        private async Task HandlePostLoginActions()
        {
            PlayerPrefs.SetString(GameConstants.AuthTokenKey, _userSession.AuthToken);
            PlayerPrefs.SetString(GameConstants.RefreshTokenKey, _userSession.RefreshToken);

            _connectionSocket.Closed += ReconnectToSocket;

            await ConnectToSocket();

            CurrentAccount = await GetUserAccount();
        }

        private async Task<IApiAccount> GetUserAccount()
        {
            try
            {
                return await _connectionClient.GetAccountAsync(_userSession);
            }
            catch (ApiResponseException e)
            {
                Debug.LogError("Error getting user account: " + e.Message);
                return null;
            }
        }

        private void ReconnectToSocket()
        {
            _connectionSocket = _connectionClient.NewSocket(useMainThread: true);
            _connectionSocket.Closed += async () => await ConnectToSocket();
        }

        private async Task ConnectToSocket()
        {
            try
            {
                if (_connectionSocket.IsConnected) return;
                await _connectionSocket.ConnectAsync(_userSession);
            }
            catch (Exception e)
            {
                Debugger.LogError($"Failed to connect to socket: {e.Message}");
            }
        }

        public async Task LoginWithDeviceId()
        {
            string deviceId = Auth.GetDeviceId();

            _userSession = await _connectionClient.AuthenticateDeviceAsync(deviceId);

            await HandlePostLoginActions();
        }

        public async Task LoginWithFacebook()
        {
            _userSession =
                await _connectionClient!.AuthenticateFacebookAsync(GameConstants.FacebookAccessToken);

            await HandlePostLoginActions();
        }

        public async Task Logout()
        {
            await _connectionClient.SessionLogoutAsync(_userSession);
        }

        public async Task DisconnectFromServer()
        {
            // if (!_connectionSocket.IsConnected) return;
            // await _connectionSocket!.CloseAsync();
        }
    }
}
#endif