using System;
using UnityEngine;

namespace KitchenKrapper
{
    public class SelectedCounterVisual : MonoBehaviour
    {
        [SerializeField] private BaseStation baseCounter;
        [SerializeField] private GameObject[] visualGameObjectArray;

        private Player localPlayer;

        private void Awake()
        {
            localPlayer = Player.LocalInstance;
            if (localPlayer != null)
            {
                SubscribeToLocalPlayer();
            }
            else
            {
                Player.OnAnyPlayerSpawned += HandlePlayerSpawned;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromLocalPlayer();
            Player.OnAnyPlayerSpawned -= HandlePlayerSpawned;
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
            localPlayer = Player.LocalInstance;
            SubscribeToLocalPlayer();
        }

        private void HandleSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
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
