using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnGameStateChanged;

    [SerializeField] private int negativeCoinAmount = -5;
    [SerializeField] private int fastDeliveryTipAmount = 5;
    [SerializeField] private int rewardAmount = 10;
    [SerializeField] private float recipeDeliveryTime = 10f;
    [SerializeField] private InputType inputType = InputType.Mobile;

    private enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        Playing,
        GameOver
    }

    private GameState gameState;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 120f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        gameState = GameState.WaitingToStart;
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer <= 0f)
                {
                    gameState = GameState.CountdownToStart;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer <= 0f)
                {
                    gameState = GameState.Playing;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.Playing:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    gameState = GameState.GameOver;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GameOver:
                break;
            default:
                break;
        }
    }

    public bool IsWaitingToStart()
    {
        return gameState == GameState.WaitingToStart;
    }

    public bool IsCountdownToStart()
    {
        return gameState == GameState.CountdownToStart;
    }

    public bool IsPlaying()
    {
        return gameState == GameState.Playing;
    }

    public bool IsGameOver()
    {
        return gameState == GameState.GameOver;
    }

    public float GetWaitingToStartTimer()
    {
        return waitingToStartTimer;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }

    public float GetGamePlayingTimer()
    {
        return gamePlayingTimer;
    }

    public int GetNegativeCoinAmount()
    {
        return negativeCoinAmount;
    }

    public int GetFastDeliveryTipAmount()
    {
        return fastDeliveryTipAmount;
    }

    public int GetRewardAmount()
    {
        return rewardAmount;
    }

    public float GetRecipeDeliveryTime()
    {
        return recipeDeliveryTime;
    }

    public InputType GetInputType()
    {
        return inputType;
    }
}
