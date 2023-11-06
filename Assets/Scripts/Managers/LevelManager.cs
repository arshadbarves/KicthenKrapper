using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KitchenKrapper
{
    public class LevelManager : NetworkBehaviour
    {
        public static LevelManager Instance { get; private set; }

        public event EventHandler<int> OnWalletAmountChanged;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnLocalPlayerReadyChanged;

        [SerializeField] private GameModeSO gameModeSO;
        [SerializeField] private PlayerController playerPrefab;
        public bool isDebugMode = false;

        private Dictionary<ulong, bool> playerReadyDictionary;

        private int walletAmount = 0;
        private int negativeCoinAmount;
        private int fastDeliveryTipAmount;
        private int rewardAmount;
        private float recipeDeliveryTime;
        private float gamePlayingTimerMax;
        private bool isLocalPlayerReady = false;
        private AudioClip music;
        private GameMode gameMode;

        private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
        private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
        private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
        [SerializeField] private AudioClipRefsSO soundEffectsAudioClipRefs;

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

            if (isDebugMode)
            {
                return;
            }

            playerReadyDictionary = new Dictionary<ulong, bool>();
        }

        private void Start()
        {
            if (!GameDataSource.PlayMultiplayer)
            {
                SpawnPlayerSinglePlayerMode();
            }

            if (IsHost)
            {
                InitializeGameMode();
            }

            SubscribeToEvents();
        }

        public override void OnDestroy()
        {
            if (isDebugMode)
            {
                return;
            }
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
            DeliveryManager.Instance.OnRecipeDeliveryFailed += DeliveryManager_OnRecipeFailed;
            DeliveryManager.Instance.OnRecipeExpired += DeliveryManager_OnRecipeExpired;
            CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
            PlayerController.OnAnyPickupObject += Player_OnPickedUpObject;
            BaseStation.OnAnyObjectPlacedOnCounter += BaseCounter_OnAnyObjectPlacedOnCounter;
            TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
        }

        private void UnsubscribeFromEvents()
        {
            DeliveryManager.Instance.OnRecipeSpawned -= DeliveryManager_OnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeDelivered -= DeliveryManager_OnRecipeDelivered;
            DeliveryManager.Instance.OnRecipeDeliveryFailed -= DeliveryManager_OnRecipeFailed;
            DeliveryManager.Instance.OnRecipeExpired -= DeliveryManager_OnRecipeExpired;
            CuttingCounter.OnAnyCut -= CuttingCounter_OnAnyCut;
            PlayerController.OnAnyPickupObject -= Player_OnPickedUpObject;
            BaseStation.OnAnyObjectPlacedOnCounter -= BaseCounter_OnAnyObjectPlacedOnCounter;
            TrashCounter.OnAnyObjectTrashed -= TrashCounter_OnAnyObjectTrashed;
        }

        private void TrashCounter_OnAnyObjectTrashed(object sender, EventArgs e)
        {
            AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.trashSounds);
        }

        private void Player_OnPickedUpObject(object sender, EventArgs e)
        {
            AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.objectPickupSounds);
        }

        private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
        {
            AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.choppingSounds);
        }

        private void DeliveryManager_OnRecipeExpired(object sender, DeliveryManager.RecipeEventArgs e)
        {
            RemoveFromWallet(GetNegativeCoinAmount());
            // AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.deliveryFailSounds);
        }

        private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
        {
            RemoveFromWallet(GetNegativeCoinAmount());
            AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.deliveryFailSounds);
        }

        private void DeliveryManager_OnRecipeDelivered(object sender, DeliveryManager.RecipeEventArgs e)
        {
            AddToWallet(GetRewardAmount());
            AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.deliverySuccessSounds);
        }

        private void DeliveryManager_OnRecipeSpawned(object sender, DeliveryManager.RecipeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BaseCounter_OnAnyObjectPlacedOnCounter(object sender, EventArgs e)
        {
            AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.objectDropSounds);
        }

        public void PlayFootstepSound(Vector3 position)
        {
            AudioManager.Instance.PlaySoundEffectAtPosition(soundEffectsAudioClipRefs.footstepSounds, position);
        }

        public void PlayCountdownSound()
        {
            AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.warningSounds);
        }

        public void PlayWarningSound()
        {
            AudioManager.Instance.PlaySoundEffect(soundEffectsAudioClipRefs.warningSounds);
        }

        private void InitializeGameMode()
        {
            if (gameModeSO != null)
            {
                negativeCoinAmount = gameModeSO.NegativeCoinAmount;
                fastDeliveryTipAmount = gameModeSO.FastDeliveryTipAmount;
                rewardAmount = gameModeSO.RewardAmount;
                recipeDeliveryTime = gameModeSO.RecipeDeliveryTime;
                gamePlayingTimerMax = gameModeSO.GamePlayingTimerMax;
                gameMode = gameModeSO.GameMode;
                music = gameModeSO.Music;
            }

            if (gameMode == GameMode.Tutorial)
            {
                gameState.Value = GameState.Playing;
            }

            AudioManager.Instance.PlayMusic(music);
        }

        public void AddToWallet(int amount)
        {
            walletAmount += amount;
            OnWalletAmountChanged?.Invoke(this, walletAmount);
        }

        public void RemoveFromWallet(int amount)
        {
            walletAmount -= amount < 0 ? 0 : amount;
            OnWalletAmountChanged?.Invoke(this, walletAmount);
        }

        public int GetWalletAmount()
        {
            return walletAmount;
        }

        public GameMode GetGameMode()
        {
            return gameMode;
        }

        public PlayerController SpawnPlayerSinglePlayerMode()
        {
            PlayerController player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            if (gameMode == GameMode.Tutorial)
            {
                player.SetIsTutorialPlayer(true);
            }
            return player;
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

            if (gameMode == GameMode.Tutorial)
            {
                Debug.Log("Tutorial mode, not updating game state");
                return;
            }

            switch (gameState.Value)
            {
                case GameState.WaitingToStart:
                case GameState.CountdownToStart:
                    countdownToStartTimer.Value -= Time.deltaTime;
                    if (countdownToStartTimer.Value <= 0f)
                    {
                        gameState.Value = GameState.Playing;
                        gamePlayingTimer.Value = gamePlayingTimerMax;
                    }
                    break;
                case GameState.Playing:
                    if (isDebugMode) return;
                    gamePlayingTimer.Value -= Time.deltaTime;
                    if (gamePlayingTimer.Value <= 0f)
                    {
                        gameState.Value = GameState.GameOver;
                    }
                    break;
                case GameState.GameOver:
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

        public bool IsLocalPlayerReadyToStart()
        {
            return isLocalPlayerReady;
        }

        public float GetCountdownToStartTimer()
        {
            return countdownToStartTimer.Value;
        }

        public float GetGamePlayingTimer(bool normalized = false)
        {
            if (normalized)
            {
                return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
            }

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
}
