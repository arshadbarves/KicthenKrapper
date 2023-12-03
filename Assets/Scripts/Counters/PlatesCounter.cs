using System;
using Managers;
using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class PlatesCounter : BaseStation
    {
        public event EventHandler OnPlateSpawned;
        public event EventHandler OnPlateRemoved;

        [SerializeField] private KitchenObjectSO platesKitchenObjectSO;

        private float spawnPlateTimer;
        private float spawnPlateTime = 5f;
        private int platesSpawned = 0;
        private int maxPlates = 5;

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }

            HandlePlateSpawning();
        }

        private void HandlePlateSpawning()
        {
            spawnPlateTimer += Time.deltaTime;
            if (spawnPlateTimer >= spawnPlateTime)
            {
                spawnPlateTimer = 0f;
                TrySpawnPlate();
            }
        }

        private void TrySpawnPlate()
        {
            if (ShouldSpawnPlate())
            {
                SpawnPlateServerRpc();
            }
        }

        private bool ShouldSpawnPlate()
        {
            return LevelManager.Instance.IsPlaying() && platesSpawned < maxPlates;
        }

        [ServerRpc]
        private void SpawnPlateServerRpc()
        {
            SpawnPlateClientRpc();
        }

        [ClientRpc]
        private void SpawnPlateClientRpc()
        {
            platesSpawned++;
            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        }

        public override void Interact(PlayerController player)
        {
            HandlePlayerInteraction(player);
        }

        private void HandlePlayerInteraction(PlayerController player)
        {
            if (!player.HasKitchenObject() && platesSpawned > 0)
            {
                CreateAndInteractPlate(player);
            }
        }

        private void CreateAndInteractPlate(PlayerController player)
        {
            KitchenObject.CreateKitchenObject(platesKitchenObjectSO, player);
            InteractServerRpc();
            StepComplete();
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractServerRpc()
        {
            InteractClientRpc();
        }

        [ClientRpc]
        private void InteractClientRpc()
        {
            platesSpawned--;
            OnPlateRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
