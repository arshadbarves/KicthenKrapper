#if NAKAMA_MULTIPLAYER
using RecipeRage.Multiplayer.NakamaServer;
#endif
using System.Threading.Tasks;
using RecipeRage.Utilities;
using UnityEngine;

namespace RecipeRage.Managers
{
    public class Startup : MonoSingleton<Startup>
    {
#if NAKAMA_MULTIPLAYER
        [SerializeField] private NakamaConnectionData nakamaConnectionData;
#endif

        protected override void Awake()
        {
            SetupGameSettings();
        }

        private async void Start()
        {
            CheckForNetworkConnection();
            // await ConnectServer();
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            // Add event listeners here
        }

        protected override void OnDestroy()
        {
            RemoveEventListeners();
        }
        
        private void OnApplicationQuit()
        {
            InternetConnectionManager.StopCheckingInternetConnection();
        }

        private void RemoveEventListeners()
        {
            // Remove event listeners here
        }

        private static void SetupGameSettings()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        private void CheckForNetworkConnection()
        {
            // _internetChecker.StopCheckingInternetConnection();
            InternetConnectionManager.StartCheckingInternetConnection(5, isConnected =>
            {
                if (!isConnected)
                {
                    // Show no internet connection popup
                    Debugger.LogWarning("No internet connection detected");
                }
                else
                {
                    // Hide no internet connection popup
                    Debugger.Log( "Internet connection detected");
                }
            });
        }
        
        private async Task ConnectServer()
        {
#if NAKAMA_MULTIPLAYER
            NakamaManager.Initialize(nakamaConnectionData);
            await NakamaManager.ConnectToServer();
#endif
        }
    }
}