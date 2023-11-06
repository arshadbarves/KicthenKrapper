using UnityEngine;
using Unity.Netcode;
namespace KitchenKrapper
{
    public class KitchenObject : NetworkBehaviour
    {
        [SerializeField] private KitchenObjectSO kitchenObjectSO;
        private IKitchenObjectParent kitchenObjectParent;
        private FollowTransform followTransform;

        protected virtual void Awake()
        {
            followTransform = GetComponent<FollowTransform>();
        }

        public KitchenObjectSO GetKitchenObjectSO()
        {
            return kitchenObjectSO;
        }

        public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
        {
            SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
        {
            SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
        }

        [ClientRpc]
        private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
        {
            kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
            IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

            if (kitchenObjectParent.HasKitchenObject())
            {
                return;
            }

            // If the kitchen object parent already has a kitchen object, destroy this one
            if (this.kitchenObjectParent != null)
            {
                this.kitchenObjectParent.RemoveKitchenObject();
            }

            this.kitchenObjectParent = kitchenObjectParent;

            kitchenObjectParent.SetKitchenObject(this);

            followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
        }

        public IKitchenObjectParent GetKitchenObjectParent()
        {
            return kitchenObjectParent;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public void RemoveKitchenObjectParent()
        {
            kitchenObjectParent.RemoveKitchenObject();
        }

        public bool TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject)
        {
            if (this is PlateKitchenObject)
            {
                plateKitchenObject = this as PlateKitchenObject;
                return true;
            }
            else
            {
                plateKitchenObject = null;
                return false;
            }
        }

        public static void CreateKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
        {
            MultiplayerManager.Instance.CreateKitchenObject(kitchenObjectSO, kitchenObjectParent);
        }

        public static void DestroyKitchenObject(KitchenObject kitchenObject)
        {
            MultiplayerManager.Instance.DestroyKitchenObject(kitchenObject);
        }
    }
}