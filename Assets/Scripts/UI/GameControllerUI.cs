using System;
using UnityEngine;

namespace KitchenKrapper
{
    public class GameControllerUI : MonoBehaviour
    {
        private LevelManager levelManager;

        private void Awake()
        {
            levelManager = LevelManager.Instance;
        }

        private void Start()
        {
            SubscribeToGameStateEvents();
            UpdateVisibility();
        }

        private void SubscribeToGameStateEvents()
        {
            if (levelManager != null)
            {
                levelManager.OnGameStateChanged += HandleGameStateChanged;
            }
        }

        private void UnsubscribeFromGameStateEvents()
        {
            if (levelManager != null)
            {
                levelManager.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        private void HandleGameStateChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (levelManager != null)
            {
                if (levelManager.IsGameOver())
                {
                    print("Game over");
                    Hide();
                }
                else
                {
                    Show();
                }
            }
        }

        public void Hide()
        {
            Debug.Log("Hiding game controller");
            gameObject.SetActive(false);
        }

        public void Show()
        {
            Debug.Log("Showing game controller");
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            UnsubscribeFromGameStateEvents();
        }
    }
}