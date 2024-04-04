using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using RecipeRage.Utilities;
using UnityEngine;

namespace RecipeRage.Multiplayer.EOS
{
    public static class EOSAuth
    {
        public static void LoginWithDeviceId()
        {
            
        }
        
        public static void LoginWithOpenID()
        {
            
        }
        
        public static void Logout()
        {
            if (PlayEveryWare.EpicOnlineServices.EOSManager.Instance.GetLocalUserId() == null)
            {
                PlayEveryWare.EpicOnlineServices.EOSManager.Instance.ClearConnectId(PlayEveryWare.EpicOnlineServices.EOSManager.Instance.GetProductUserId());
                ResetPlayerAuth();
                Debug.Log("Logout Successful. [No User]");
                return;
            }

            PlayEveryWare.EpicOnlineServices.EOSManager.Instance.StartLogout(PlayEveryWare.EpicOnlineServices.EOSManager.Instance.GetLocalUserId(), (ref LogoutCallbackInfo data) =>
            {
                if (data.ResultCode == Result.Success)
                {
                    Debug.Log("Logout Successful. [" + data.ResultCode + "]");
                    ResetPlayerAuth();
                }
                else
                {
                    Debug.Log("Logout Failed. [" + data.ResultCode + "]");
                }
            });
        }
        
        private static void ResetPlayerAuth()
        {
            var authInterface = PlayEveryWare.EpicOnlineServices.EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
            var options = new DeletePersistentAuthOptions();

            authInterface.DeletePersistentAuth(ref options, null, (ref DeletePersistentAuthCallbackInfo deletePersistentAuthCallbackInfo) =>
            {
                Debugger.Log(deletePersistentAuthCallbackInfo.ResultCode == Result.Success
                    ? "Persistent auth deleted"
                    : "Persistent auth not deleted");
            });

            var connectInterface = PlayEveryWare.EpicOnlineServices.EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
            var connectOptions = new DeleteDeviceIdOptions();

            connectInterface.DeleteDeviceId(ref connectOptions, null, (ref DeleteDeviceIdCallbackInfo deleteDeviceIdCallbackInfo) =>
            {
                Debugger.Log(deleteDeviceIdCallbackInfo.ResultCode == Result.Success
                    ? "Device ID deleted"
                    : "Device ID not deleted");
            });
        }
    }
}