using KitchenKrapper;
using Player;
using UnityEngine;

namespace Counters
{
    public class SelectedCounterVisual : MonoBehaviour
    {
        [SerializeField] private BaseStation baseCounter;
        [SerializeField] private GameObject[] visualGameObjectArray;

        private PlayerController _localPlayer;

        private void Awake()
        {
            _localPlayer = PlayerController.LocalInstance;
            if (_localPlayer != null)
            {
                SubscribeToLocalPlayer();
            }
            else
            {
                BasePlayer.OnPlayerSpawned += HandlePlayerSpawnedHandler;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromLocalPlayer();
            BasePlayer.OnPlayerSpawned -= HandlePlayerSpawnedHandler;
        }

        private void SubscribeToLocalPlayer()
        {
            _localPlayer.OnSelectedCounterChanged += HandleSelectedCounterChangedHandler;
        }

        private void UnsubscribeFromLocalPlayer()
        {
            if (_localPlayer != null)
            {
                _localPlayer.OnSelectedCounterChanged -= HandleSelectedCounterChangedHandler;
            }
        }

        private void HandlePlayerSpawnedHandler()
        {
            _localPlayer = PlayerController.LocalInstance;
            SubscribeToLocalPlayer();
        }

        private void HandleSelectedCounterChangedHandler(BasePlayer.OnSelectedCounterChangedEventArgs onSelectedCounterChangedEventArgs)
        {
            if (onSelectedCounterChangedEventArgs.SelectedCounter == baseCounter)
            {
                ShowVisual();
            }
            else
            {
                HideVisual();
            }
        }

        private void ShowVisual()
        {
            foreach (var visualGameObject in visualGameObjectArray)
            {
                visualGameObject.SetActive(true);
            }
        }

        private void HideVisual()
        {
            foreach (var visualGameObject in visualGameObjectArray)
            {
                visualGameObject.SetActive(false);
            }
        }
    }
}
