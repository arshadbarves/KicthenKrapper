using System;
using Unity.Netcode;

namespace KitchenKrapper
{
    public class TrashCounter : BaseStation
    {
        public static event EventHandler OnAnyObjectTrashed;

        public static new void ResetStaticData()
        {
            OnAnyObjectTrashed = null;
        }

        public override void Interact(PlayerController player)
        {
            if (player.HasKitchenObject())
            {
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                InteractServerRpc();

                StepComplete();
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
            OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}
