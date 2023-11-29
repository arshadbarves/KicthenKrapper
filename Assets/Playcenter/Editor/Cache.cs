using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using PlayEveryWare.EpicOnlineServices;
using UnityEditor;
using UnityEngine;

namespace PlayCenter.Editor
{
    public class Cache : UnityEditor.Editor
    {
        [MenuItem(Constants.Menu + "Cache/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
        
        [MenuItem(Constants.Menu + "Cache/Reset Game")]
        public static void ResetGame()
        {
            var authInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
            var options = new DeletePersistentAuthOptions();

            authInterface.DeletePersistentAuth(ref options, null, (ref DeletePersistentAuthCallbackInfo deletePersistentAuthCallbackInfo) =>
            {
                Debug.Log(deletePersistentAuthCallbackInfo.ResultCode == Result.Success
                    ? "Persistent auth deleted"
                    : "Persistent auth not deleted");
            });

            var connectInterface = EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
            var connectOptions = new DeleteDeviceIdOptions();

            connectInterface.DeleteDeviceId(ref connectOptions, null, (ref DeleteDeviceIdCallbackInfo deleteDeviceIdCallbackInfo) =>
            {
                Debug.Log(deleteDeviceIdCallbackInfo.ResultCode == Result.Success
                    ? "Device ID deleted"
                    : "Device ID not deleted");
            });
        }
        
        [MenuItem(Constants.Menu + "Cache/Reset All")]
        public static void ResetAll()
        {
            ResetGame();
            ClearPlayerPrefs();
        }
    }
}