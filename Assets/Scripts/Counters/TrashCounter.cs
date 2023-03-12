using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed;

    public static new void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            InteractServerRpc();
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