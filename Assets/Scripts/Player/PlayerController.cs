using System;
using System.Collections.Generic;
using Cinemachine;
using Epic.OnlineServices;
using KitchenKrapper;
using Managers;
using PlayEveryWare.EpicOnlineServices.Samples.Network;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerController : BasePlayer
    {
        public static PlayerController LocalInstance { get; private set; }

        public static void ResetStaticData()
        {
            LocalInstance = null;
        }

        [SerializeField] private List<Vector3> spawnPoints;

        protected override void Start()
        {
            base.Start();
            GameInput.Instance.OnInteractAction += GameInput_OnInteractActionHandler;
            GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateActionHandler;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameInput.Instance.OnInteractAction -= GameInput_OnInteractActionHandler;
            GameInput.Instance.OnInteractAlternateAction -= GameInput_OnInteractAlternateActionHandler;
        }

        public override void OnNetworkSpawn()
        {
            SpawnPlayer();
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }

            if (IsOwner)
            {
                displayNameText.name = GameManager.Instance.GetPlayerName();
                displayNameText.color = Color.green;

                // Get the Virtual Camera and follow the player
                var virtualCameraObject = FindFirstObjectByType<CinemachineVirtualCamera>();
                if (virtualCameraObject == null) return;
                var playerTransform = transform;
                virtualCameraObject.Follow = playerTransform;
                virtualCameraObject.LookAt = playerTransform;
            }
            else
            {
                displayNameText.name = MultiplayerManager.Instance.GetPlayerDataFromPlayerId(OwnerClientId).playerName
                    .ToString();
                displayNameText.color = Color.red;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
            }
        }

        private void SpawnPlayer()
        {
            if (IsOwner)
            {
                LocalInstance = this;
            }

            transform.position = GetRandomSpawnPoint();
            PlayerSpawned();
        }

        // Spawn on random SpawnPoint which is not occupied by another player
        private Vector3 GetRandomSpawnPoint()
        {
            List<Vector3> freeSpawnPoints = new List<Vector3>(spawnPoints);
            foreach (PlayerController player in FindObjectsOfType<PlayerController>())
            {
                if (player != this)
                {
                    freeSpawnPoints.Remove(player.transform.position);
                }
            }

            return freeSpawnPoints[UnityEngine.Random.Range(0, freeSpawnPoints.Count)];
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == OwnerClientId && IsServer && HasKitchenObject())
            {
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
            }
        }

        private void GameInput_OnInteractAlternateActionHandler(object sender, EventArgs e)
        {
            if (!LevelManager.Instance.isDebugMode)
            {
                if (!TutorialManager.Instance && !LevelManager.Instance.IsPlaying()) return;
            }

            if (SelectedCounter != null && GrabbedStationObject == null)
            {
                if (LevelManager.Instance.IsPlaying() && !LevelManager.Instance.isDebugMode)
                {
                    SelectedCounter.InteractAlternate(this);
                }
                else
                {
                    GrabStationObject(SelectedCounter);
                }
            }
            else
            {
                if (HasStationObject())
                {
                    print("GameInput_OnInteractAlternateActionHandler: HasStationObject");
                    PlaceStationObject();
                }
            }
        }

        private void PlaceStationObject()
        {
            RemoveKitchenObject();
            if (PlacementSystem.Instance.PlaceStationObject(GrabbedStationObject))
            {
                GrabbedStationObject = null;
            }
        }

        private void GrabStationObject(BaseStation station)
        {
            GrabbedStationObject = station;
            PlacementSystem.Instance.StartPlacingStation(this, GrabbedStationObject);
            GrabbedStationObject.PickStationParent(this);
        }

        private bool HasStationObject()
        {
            return GrabbedStationObject != null;
        }

        private void GameInput_OnInteractActionHandler(object sender, EventArgs e)
        {
            if (!LevelManager.Instance.IsPlaying()) return;
            SelectedCounter.Interact(this);
        }

        protected override void Update()
        {
            if (!IsOwner) return; // Only the owner of the player can move it
            base.Update();
        }

        public static void SetNetworkHostId(ProductUserId userId)
        {
            var transportLayer = NetworkManager.Singleton.GetComponent<EOSTransport>();
            if (transportLayer != null)
            {
                transportLayer.ServerUserIdToConnectTo = userId;
            }
        }
    }
}