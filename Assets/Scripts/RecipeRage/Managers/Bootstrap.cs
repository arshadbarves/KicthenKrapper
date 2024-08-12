#if NAKAMA_MULTIPLAYER
using RecipeRage.Multiplayer.NakamaServer;
#endif
using System.Threading.Tasks;
using RecipeRage.NakamaServer;
using RecipeRage.Utilities;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RecipeRage.Managers
{
    public class Bootstrap : MonoSingleton<Bootstrap>
    {
#if NAKAMA_MULTIPLAYER
        [SerializeField] private NakamaConnectionData nakamaConnectionData;
#endif

        private const string NoInternetConnectionMessage = "No internet connection detected. Please check your connection and try again.";
        public NakamaConnectionData NakamaConnectionData { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            NakamaConnectionData = nakamaConnectionData;
            SetupGameSettings();
        }

        private void Start()
        {
            CheckForNetworkConnection();
        }

        private void OnApplicationQuit()
        {
            InternetConnectionManager.StopCheckingInternetConnection();
            DisconnectFromServer();
        }

        private static void SetupGameSettings()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        private static void CheckForNetworkConnection()
        {
            InternetConnectionManager.StartCheckingInternetConnection(5, isConnected =>
            {
                switch (isConnected)
                {
                    case false when !UIManager.Instance!.IsWarningPopupShown():
                        UIManager.Instance.ShowWarningPopup(NoInternetConnectionMessage, null, "No Internet Connection", false, "Ok");
                        break;
                    case true:
                        UIManager.Instance.HideWarningPopup();
                        break;
                }
            });
        }
        
        private static void DisconnectFromServer()
        {
#if NAKAMA_MULTIPLAYER
            if(NakamaManager.Instance == null) return;
            NakamaManager.Instance.DisconnectFromServer().Wait();
#endif
        }
    }
}