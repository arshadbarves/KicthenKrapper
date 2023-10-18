using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace KitchenKrapper
{
    // non-UI logic for HomeScreen
    public class HomeScreenController : MonoBehaviour
    {
        // events
        public static event Action<LevelSO> ShowLevelInfo;
        public static event Action MainMenuExited;

        [Header("Level Data")]
        [SerializeField] LevelSO m_LevelData;

        void OnEnable()
        {
            HomeScreen.PlayButtonClicked += OnPlayGameLevel;
        }

        void OnDisable()
        {
            HomeScreen.PlayButtonClicked -= OnPlayGameLevel;
        }

        void Start()
        {
            ShowLevelInfo?.Invoke(m_LevelData);
        }

        // scene-management methods
        public void OnPlayGameLevel()
        {
            if (m_LevelData == null)
                return;

            MainMenuExited?.Invoke();

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                if (LobbyManager.Instance.GetCurrentLobby() == null)
                    LobbyManager.Instance.StartMatchmaking();
                else
                    MatchmakingManager.Instance.StartMatchmaking(false);
        }
    }
}
