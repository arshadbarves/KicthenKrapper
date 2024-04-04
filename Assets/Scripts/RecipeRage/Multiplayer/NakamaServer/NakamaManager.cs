#if NAKAMA_MULTIPLAYER
using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

namespace RecipeRage.Multiplayer.NakamaServer
{
    public static class NakamaManager
    {
        private static NakamaConnectionData _nakamaConnectionData;

        private const string SessionPrefName = "nakama.session";
        private const string DeviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";


        private static IClient _connectionClient;
        private static ISession _userSession;
        private static ISocket _connectionSocket;
        private static IApiAccount _currentAccount;


        private static string _currentMatchmakingTicket;
        private static string _currentMatchId;

        public static void Initialize(NakamaConnectionData nakamaConnection)
        {
            _nakamaConnectionData = nakamaConnection;
        }


        public static async Task ConnectToServer()
        {
            _connectionClient = new Client(_nakamaConnectionData.Scheme, _nakamaConnectionData.Host,
                _nakamaConnectionData.Port, _nakamaConnectionData.ServerKey, UnityWebRequestAdapter.Instance);
            _connectionClient.Timeout = 5000; // 5 seconds

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
                        PlayerPrefs.SetString(GameConstants.AuthTokenKey, _userSession.AuthToken);
                    }
                    catch (ApiResponseException)
                    {
                        // TODO: Show Login UI and get user to login again and continue from here.
                        PlayerPrefs.SetString(GameConstants.RefreshTokenKey, _userSession.RefreshToken);
                    }
                }
            }
            else
            {
                _userSession = await _connectionClient.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
                PlayerPrefs.SetString(GameConstants.AuthTokenKey, _userSession.AuthToken);
                PlayerPrefs.SetString(GameConstants.RefreshTokenKey, _userSession.RefreshToken);
            }


            // Try to restore an existing session from PlayerPrefs.

            if (!string.IsNullOrEmpty(authToken))
            {
                var session = Session.Restore(authToken);
                if (!session.IsExpired)
                {
                    _userSession = session;
                }
            }

            // If we weren't able to restore an existing session, authenticate to create a new user session.
            if (_userSession == null)
            {
                string deviceId;

                // If we've already stored a device identifier in PlayerPrefs then use that.
                if (PlayerPrefs.HasKey(DeviceIdentifierPrefName))
                {
                    deviceId = PlayerPrefs.GetString(DeviceIdentifierPrefName);
                }
                else
                {
                    // If we've reach this point, get the device's unique identifier or generate a unique one.
                    deviceId = SystemInfo.deviceUniqueIdentifier;
                    if (deviceId == SystemInfo.unsupportedIdentifier)
                    {
                        deviceId = Guid.NewGuid().ToString();
                    }

                    // Store the device identifier to ensure we use the same one each time from now on.
                    PlayerPrefs.SetString(DeviceIdentifierPrefName, deviceId);
                }

                // Use Nakama Device authentication to create a new session using the device identifier.
                _userSession = await _connectionClient.AuthenticateDeviceAsync(deviceId);

                // Store the auth token that comes back so that we can restore the session later if necessary.
                PlayerPrefs.SetString(SessionPrefName, _userSession.AuthToken);
            }

            // Open a new Socket for realtime communication.
            _connectionSocket = _connectionClient.NewSocket();
            await _connectionSocket.ConnectAsync(_userSession, true);
        }

        public static async Task LoginWithDeviceId()
        {
            string deviceId = PlayerPrefs.GetString(GameConstants.DeviceIdKey);
            if (string.IsNullOrWhiteSpace(deviceId))
            {
#if UNITY_WEBGL
                deviceId = Guid.NewGuid().ToString();
#else
                deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
                PlayerPrefs.SetString(GameConstants.DeviceIdKey, deviceId);
            }

            _userSession = await _connectionClient.AuthenticateDeviceAsync(deviceId);
            
            PlayerPrefs.SetString(GameConstants.AuthTokenKey, _userSession.AuthToken);
            PlayerPrefs.SetString(GameConstants.RefreshTokenKey, _userSession.RefreshToken);
        }

        public static async Task LoginWithFacebook()
        {
            _userSession = await _connectionClient.AuthenticateFacebookAsync("facebook-access-token");
            
            PlayerPrefs.SetString(GameConstants.AuthTokenKey, _userSession.AuthToken);
            PlayerPrefs.SetString(GameConstants.RefreshTokenKey, _userSession.RefreshToken);
        }

        public static async Task Logout()
        {
            await _connectionClient.SessionLogoutAsync(_userSession);
        }
    }
}
#endif