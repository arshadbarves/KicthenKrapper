using System;
using Managers;
using SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Utils.Enums;

namespace KitchenKrapper
{
    public class TutorialOverUI : MonoBehaviour
    {
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void Start()
        {
            LevelManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
            Hide();
        }

        private void GameManager_OnGameStateChanged(object sender, EventArgs e)
        {
            if (LevelManager.Instance.IsGameOver())
            {
                Show();
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
            SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), false);
        }
    }
}