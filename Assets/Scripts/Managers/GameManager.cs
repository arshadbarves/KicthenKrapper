using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnGameStateChanged;
    public event EventHandler OnLocalPlayerReadyChanged;

    [SerializeField] private GameModeSO gameModeSO;
    [SerializeField] private Player playerPrefab;
    private int negativeCoinAmount;
    private int fastDeliveryTipAmount;
    private int rewardAmount;
    private float recipeDeliveryTime;
    private float gamePlayingTimerMax;
    private bool isLocalPlayerReady = false;
    private Dictionary<ulong, bool> playerReadyDictionary;

    private enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        Playing,
        GameOver
    }

    private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);


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

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        if (!GameDataSource.playMultiplayer)
        {
            var playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }

        if (gameModeSO != null)
        {
            negativeCoinAmount = gameModeSO.NegativeCoinAmount;
            fastDeliveryTipAmount = gameModeSO.FastDeliveryTipAmount;
            rewardAmount = gameModeSO.RewardAmount;
            recipeDeliveryTime = gameModeSO.RecipeDeliveryTime;
            gamePlayingTimerMax = gameModeSO.GamePlayingTimerMax;
        }
    }

    public override void OnNetworkSpawn()
    {
        gameState.OnValueChanged += GameState_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    public override void OnNetworkDespawn()
    {
        gameState.OnValueChanged -= GameState_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in clientsCompleted)
        {
            var playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkObject playerNetworkObject = playerObject.GetComponent<NetworkObject>();
            playerNetworkObject.SpawnAsPlayerObject(clientId, true);
        }
    }

    private void GameState_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(bool isReady = false, ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = isReady;

        if (playerReadyDictionary.Count == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            bool allPlayersReady = true;
            foreach (var playerReady in playerReadyDictionary)
            {
                if (!playerReady.Value)
                {
                    allPlayersReady = false;
                    break;
                }
            }

            if (allPlayersReady)
            {
                gameState.Value = GameState.CountdownToStart;
            }
        }
    }

    private void Update()
    {
        if (!IsServer) return; // Only server should update game state

        switch (gameState.Value)
        {
            case GameState.WaitingToStart:
                gameState.Value = GameState.CountdownToStart;
                break;
            case GameState.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value <= 0f)
                {
                    gameState.Value = GameState.Playing;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case GameState.Playing:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value <= 0f)
                {
                    gameState.Value = GameState.GameOver;
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
        return gameState.Value == GameState.WaitingToStart;
    }

    public bool IsCountdownToStart()
    {
        return gameState.Value == GameState.CountdownToStart;
    }

    public bool IsPlaying()
    {
        return gameState.Value == GameState.Playing;
    }

    public bool IsGameOver()
    {
        return gameState.Value == GameState.GameOver;
    }

    public bool isLocalPlayerReadyToStart()
    {
        return isLocalPlayerReady;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    public float GetGamePlayingTimer()
    {
        return gamePlayingTimer.Value;
    }

    public int GetNegativeCoinAmount()
    {
        return negativeCoinAmount * -1;
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
}
