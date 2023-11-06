using System;
using UnityEngine;

namespace KitchenKrapper
{
    public class SelectedCounterVisual : MonoBehaviour
    {
        [SerializeField] private BaseStation baseCounter;
        [SerializeField] private GameObject[] visualGameObjectArray;

        private PlayerController localPlayer;

        private void Awake()
        {
            localPlayer = PlayerController.LocalInstance;
            if (localPlayer != null)
            {
                SubscribeToLocalPlayer();
            }
            else
            {
                PlayerController.OnAnyPlayerSpawned += HandlePlayerSpawned;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromLocalPlayer();
            PlayerController.OnAnyPlayerSpawned -= HandlePlayerSpawned;
        }

        private void SubscribeToLocalPlayer()
        {
            localPlayer.OnSelectedCounterChanged += HandleSelectedCounterChanged;
        }

        private void UnsubscribeFromLocalPlayer()
        {
            if (localPlayer != null)
            {
                localPlayer.OnSelectedCounterChanged -= HandleSelectedCounterChanged;
            }
        }

        private void HandlePlayerSpawned(object sender, EventArgs e)
        {
            localPlayer = PlayerController.LocalInstance;
            SubscribeToLocalPlayer();
        }

        private void HandleSelectedCounterChanged(object sender, PlayerController.OnSelectedCounterChangedEventArgs e)
        {
            if (e.selectedCounter == baseCounter)
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
            foreach (GameObject visualGameObject in visualGameObjectArray)
            {
                visualGameObject.SetActive(true);
            }
        }

        private void HideVisual()
        {
            foreach (GameObject visualGameObject in visualGameObjectArray)
            {
                visualGameObject.SetActive(false);
            }
        }
    }
}
