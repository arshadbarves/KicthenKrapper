using System;
using Unity.Netcode;
using DG.Tweening;
using UnityEngine;

namespace KitchenKrapper
{
    public class BaseStation : NetworkBehaviour, IKitchenObjectParent
    {
        public static event EventHandler OnAnyObjectPlacedOnCounter;
        private IKitchenObjectParent kitchenObjectParent;

        [SerializeField] private Transform counterTopPoint;
        private int tutorialStepIndex;
        private KitchenObject kitchenObject;
        private FollowTransform followTransform;

        public int TutorialStepIndex => tutorialStepIndex;

        protected virtual void Awake()
        {
            followTransform = GetComponent<FollowTransform>();
            Debug.Log(followTransform);
        }

        public static void ResetStaticData()
        {
            OnAnyObjectPlacedOnCounter = null;
        }

        public Transform GetKitchenObjectFollowTransform() => counterTopPoint;

        public KitchenObject GetKitchenObject() => kitchenObject;

        public void SetKitchenObject(KitchenObject kitchenObj)
        {
            kitchenObject = kitchenObj;
            OnAnyObjectPlacedOnCounter?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveKitchenObject() => kitchenObject = null;

        public bool HasKitchenObject() => kitchenObject != null;

        public NetworkObject GetNetworkObject() => NetworkObject;

        public void StepComplete()
        {
            TutorialManager.Instance?.CompleteTutorialStep(tutorialStepIndex);
        }

        public virtual void Interact(PlayerController player)
        {
            Debug.LogException(new Exception("Interact() not implemented in " + GetType().Name));
        }

        public virtual void InteractAlternate(PlayerController player)
        {
            // Optional implementation
        }

        public int GetTutorialStepIndex() => tutorialStepIndex;

        public void SetTutorialStepIndex(int stepIndex) => tutorialStepIndex = stepIndex;

        public void PickStationParent(IKitchenObjectParent parent)
        {
            PickStationServerRpc(parent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void PickStationServerRpc(NetworkObjectReference stationNetworkObjectReference)
        {
            PickStationClientRpc(stationNetworkObjectReference);
        }

        [ClientRpc]
        private void PickStationClientRpc(NetworkObjectReference stationParentNetworkObjectReference)
        {
            if (!stationParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject))
                return;
            kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

            if (kitchenObjectParent.HasKitchenObject())
            {
                return;
            }

            // If the kitchen object parent already has a kitchen object, destroy this one
            kitchenObjectParent?.RemoveKitchenObject();

            kitchenObjectParent = kitchenObjectParent;
            transform.localScale *= 0.5f;
            GetComponent<Collider>().enabled = false;

            followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
        }

        // [ClientRpc]
        // private void PickStationClientRpc(NetworkObjectReference stationParentNetworkObjectReference)
        // {
        //     stationParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        //     IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        //     if (kitchenObjectParent.HasKitchenObject())
        //     {
        //         return;
        //     }
        //     // If the kitchen object parent already has a kitchen object, destroy this one	
        //     this.kitchenObjectParent?.RemoveKitchenObject();
        //     this.kitchenObjectParent = kitchenObjectParent;
        //     // set the size to small	
        //     this.transform.localScale = transform.localScale * 0.5f;
        //     this.transform.GetComponent<Collider>().enabled = false;
        //     followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
        // }

        public void DropStationParent(Vector3 position)
        {
            DropStationServerRpc(position);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DropStationServerRpc(Vector3 position)
        {
            DropStationClientRpc(position);
        }

        [ClientRpc]
        private void DropStationClientRpc(Vector3 position)
        {
            kitchenObjectParent?.RemoveKitchenObject();

            kitchenObjectParent = null;
            followTransform.SetTargetTransform(null);
            WobbleEffect();
            transform.position = position;
            transform.rotation = Quaternion.identity;
            transform.localScale *= 2f;
            GetComponent<Collider>().enabled = true;
        }

        public void WobbleEffect()
        {
            transform.DOShakeScale(0.3f, 0.2f, 10, 90, false);
        }
    }
}