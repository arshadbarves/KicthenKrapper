using System;
using UnityEngine;

public class GameControllerUI : MonoBehaviour
{

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        }
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Hide()
    {
        print("hiding game controller");
        gameObject.SetActive(false);
    }

    public void Show()
    {
        print("showing game controller");
        gameObject.SetActive(true);
    }
}
