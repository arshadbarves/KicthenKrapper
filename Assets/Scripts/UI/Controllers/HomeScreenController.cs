using System;
using KitchenKrapper;
using Multiplayer.EOS;
using UI.Screen;
using UnityEngine;

namespace UI.Controllers
{
    // non-UI logic for HomeScreen
    public class HomeScreenController : MonoBehaviour
    {
        // events
        public static event Action<LevelSO> OnShowLevelInfo;
        public static event Action OnMainMenuExited;

        [Header("Level Data")] [SerializeField]
        private LevelSO levelData;

        private void OnEnable()
        {
            HomeScreen.PlayButtonClicked += OnPlayGameLevel;
        }

        private void OnDisable()
        {
            HomeScreen.PlayButtonClicked -= OnPlayGameLevel;
        }

        private void Start()
        {
            OnShowLevelInfo?.Invoke(levelData);
        }

        // scene-management methods
        private void OnPlayGameLevel()
        {
            if (levelData == null) return;
            OnMainMenuExited?.Invoke();
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (LobbyManager.Instance.GetCurrentLobby() != null)
                LobbyManager.Instance.StartMatchmaking();
            else
                MatchmakingManager.Instance.StartMatchmaking(MatchType.Ranked);
        }
    }
}