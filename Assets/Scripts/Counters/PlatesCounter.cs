using System;
using Unity.Netcode;
using UnityEngine;

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

        SpawnPlate();
    }

    private void SpawnPlate()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer >= spawnPlateTime)
        {
            spawnPlateTimer = 0f;
            if (LevelManager.Instance.IsPlaying() && platesSpawned < maxPlates)
            {
                SpawnPlateServerRpc();
            }
        }
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

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawned > 0)
            {
                KitchenObject.CreateKitchenObject(platesKitchenObjectSO, player);
                InteractServerRpc();

                StepComplete();
            }
        }
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
