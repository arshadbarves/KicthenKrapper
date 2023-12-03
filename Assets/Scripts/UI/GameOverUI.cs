using System;
using Managers;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class GameOverUI : MonoBehaviour
    {
        private const string GAME_OVER_ANIMATION_TRIGGER = "GameOver";
        [SerializeField] private TextMeshProUGUI recipesDeliveredText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button quitButton;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void Start()
        {
            SubscribeToGameStateEvents();
            Hide();
        }

        private void SubscribeToGameStateEvents()
        {
            LevelManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }

        private void UnsubscribeFromGameStateEvents()
        {
            LevelManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (LevelManager.Instance.IsGameOver())
            {
                Show();
                animator.SetTrigger(GAME_OVER_ANIMATION_TRIGGER);
                recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulDeliveryCount().ToString();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnQuitButtonClicked()
        {
            ShutdownNetworkManager();
            LoadScene(SceneType.MainMenu.ToString());
        }

        private void LoadScene(string sceneName)
        {
            SceneLoaderWrapper.Instance.LoadScene(sceneName, false);
        }

        private void ShutdownNetworkManager()
        {
            NetworkManager.Singleton.Shutdown();
        }

        private void OnDestroy()
        {
            UnsubscribeFromGameStateEvents();
        }
    }
}