using System;
using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class ContainerCounter : BaseStation
    {
        public event EventHandler OnContainerCounterInteracted;

        [SerializeField] private KitchenObjectSO kitchenObjectSO;

        public override void Interact(Player player)
        {
            HandleInteract(player);
        }

        private void HandleInteract(Player player)
        {
            if (!player.HasKitchenObject())
            {
                CreateAndInteractKitchenObject(player);
            }
        }

        private void CreateAndInteractKitchenObject(Player player)
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