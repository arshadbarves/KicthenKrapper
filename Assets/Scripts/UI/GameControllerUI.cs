using System;
using UnityEngine;

public class GameControllerUI : MonoBehaviour
{
    private LevelManager gameManager;

    private void Awake()
    {
        gameManager = LevelManager.Instance;
    }

    private void Start()
    {
        SubscribeToGameStateEvents();
        UpdateVisibility();
    }

    private void SubscribeToGameStateEvents()
    {
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void UnsubscribeFromGameStateEvents()
    {
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(object sender, EventArgs e)
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (gameManager != null)
        {
            if (gameManager.IsGameOver())
            {
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
