using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;
using TMPro;
using QFSW.QC.Actions;

namespace KitchenKrapper
{
    public class Debugger : MonoBehaviour
    {
        [SerializeField] private bool m_resetPlayerPrefs = false;
        [SerializeField] private bool m_resetGame = false;
        [SerializeField] private TextMeshProUGUI m_debugFPS;
        [SerializeField] private Canvas m_debugCanvas;
        [SerializeField] private bool m_showDebugCanvas = false;
        private float deltaTime_FPS;

        void Awake()
        {
            // Check if the game is in debug mode. If it is, then delete all the player prefs.
            if (m_resetPlayerPrefs)
            {
                // Clear the Player Prefs.
                PlayerPrefs.DeleteAll();
            }

            // Check if the game is in debug mode. If it is, then reset the game.
            if (m_resetGame)
            {
                var authInterface = EOSManager.Instance.GetEOSPlatformInterface().GetAuthInterface();
                var options = new Epic.OnlineServices.Auth.DeletePersistentAuthOptions();

                authInterface.DeletePersistentAuth(ref options, null, (ref DeletePersistentAuthCallbackInfo deletePersistentAuthCallbackInfo) =>
                {
                    if (deletePersistentAuthCallbackInfo.ResultCode == Result.Success)
                    {
                        Debug.Log("Persistent auth deleted");
                    }
                    else
                    {
                        Debug.Log("Persistent auth not deleted");
                    }
                });

                var connectInterface = EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
                var connectOptions = new Epic.OnlineServices.Connect.DeleteDeviceIdOptions();

                connectInterface.DeleteDeviceId(ref connectOptions, null, (ref DeleteDeviceIdCallbackInfo deleteDeviceIdCallbackInfo) =>
                {
                    if (deleteDeviceIdCallbackInfo.ResultCode == Result.Success)
                    {
                        Debug.Log("Device ID deleted");
                    }
                    else
                    {
                        Debug.Log("Device ID not deleted");
                    }
                });
            }
        }

        private void Start()
        {
            if (m_showDebugCanvas)
                m_debugCanvas.gameObject.SetActive(true);
            else
                m_debugCanvas.gameObject.SetActive(false);
        }

        private void Update()
        {
            // FPS
            if (m_showDebugCanvas)
            {
                deltaTime_FPS += (Time.deltaTime - deltaTime_FPS) * 0.1f;
                float fps = 1.0f / deltaTime_FPS;
                m_debugFPS.text = Mathf.Ceil(fps).ToString() + " FPS";
            }

            // Get Key R
            // if (Input.GetKeyDown(KeyCode.R))
            // {
            //     EOSAuth.Instance.Login();
            // }

        }
    }
}