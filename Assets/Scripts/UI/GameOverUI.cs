using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        playAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
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
        LoadScene(SceneType.MainMenu.ToString());
    }

    private void OnPlayAgainButtonClicked()
    {
        LoadScene(GameDataSource.Instance.GetCurrentMap().ToString());
    }

    private void LoadScene(string sceneName)
    {
        SceneLoaderWrapper.Instance.LoadScene(sceneName, GameDataSource.Instance.UseNetworkSceneManager());
    }
}
