using System;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class ContainerCounter : BaseStation
    {
        public event EventHandler OnContainerCounterInteracted;

        [SerializeField] private KitchenObjectSO kitchenObjectSO;

        public override void Interact(PlayerController player)
        {
            HandleInteract(player);
        }

        private void HandleInteract(PlayerController player)
        {
            if (!player.HasKitchenObject())
            {
                CreateAndInteractKitchenObject(player);
            }
        }

        private void CreateAndInteractKitchenObject(PlayerController player)
        {
            KitchenObject.CreateKitchenObject(kitchenObjectSO, player);
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
            OnContainerCounterInteracted?.Invoke(this, EventArgs.Empty);
        }
    }
}