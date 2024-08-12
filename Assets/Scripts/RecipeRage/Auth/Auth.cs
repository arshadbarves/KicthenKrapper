#if EOS_MULTIPLAYER
#endif
using System.Threading.Tasks;
using RecipeRage.Multiplayer.EOS;
using RecipeRage.NakamaServer;
using UnityEngine;

namespace RecipeRage
{
    public enum AuthType
    {
        DeviceId,
        Facebook
    }
    public static class Auth
    {
        public static async Task StartLogin(AuthType authType)
        {
            switch (authType)
            {
                case AuthType.DeviceId:
#if EOS_MULTIPLAYER
                    await EOSManager.LoginWithDeviceId();
#endif
#if NAKAMA_MULTIPLAYER
                    await NakamaManager.Instance.LoginWithDeviceId();
                    break;
#endif
                case AuthType.Facebook:
#if EOS_MULTIPLAYER
                    await EOSManager.LoginWithOpenID();
#endif
#if NAKAMA_MULTIPLAYER
                    await NakamaManager.Instance.LoginWithFacebook();
                    break;
#endif
                default:
                    return;
            }
        }

        public static async Task StartLogout()
        {
#if EOS_MULTIPLAYER
            await EOSManager.Logout();
#endif
#if NAKAMA_MULTIPLAYER

            await NakamaManager.Instance.Logout();
#endif
        }

        public static string GetDeviceId()
        {
            string deviceId = PlayerPrefs.GetString(GameConstants.DeviceIdKey, null);
            
            if (string.IsNullOrWhiteSpace(deviceId))
            {
#if UNITY_WEBGL
                deviceId = Guid.NewGuid().ToString();
#else
                deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
                PlayerPrefs.SetString(GameConstants.DeviceIdKey, deviceId);
            }

            return deviceId;
        }
    }
}