using System;
using Unity.Netcode;
using DG.Tweening;
using UnityEngine;

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
        print(followTransform);
    }

    public static void ResetStaticData()
    {
        OnAnyObjectPlacedOnCounter = null;
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnAnyObjectPlacedOnCounter?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RemoveKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public void StepComplete()
    {
        TutorialManager.Instance?.CompleteTutorialStep(TutorialStepIndex);
    }

    public virtual void Interact(Player player)
    {
        Debug.LogException(new System.Exception("Interact() not implemented in " + GetType().Name));
    }

    public virtual void InteractAlternate(Player player)
    {
        // Optional implementation
    }

    public int GetTutorialStepIndex()
    {
        return tutorialStepIndex;
    }

    public void SetTutorialStepIndex(int tutorialStepIndex)
    {
        this.tutorialStepIndex = tutorialStepIndex;
    }

    public void PickStationParent(IKitchenObjectParent kitchenObjectParent)
    {
        PickStationServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickStationServerRpc(NetworkObjectReference stationNetworkObjectReference)
    {
        PickStationClientRpc(stationNetworkObjectReference);
    }

    [ClientRpc]
    private void PickStationClientRpc(NetworkObjectReference stationParentNetworkObjectReference)
    {
        stationParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (kitchenObjectParent.HasKitchenObject())
        {
            return;
        }

        // If the kitchen object parent already has a kitchen object, destroy this one
        this.kitchenObjectParent?.RemoveKitchenObject();

        this.kitchenObjectParent = kitchenObjectParent;
        // set the size to small
        this.transform.localScale = transform.localScale * 0.5f;

        this.transform.GetComponent<Collider>().enabled = false;

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }

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
        this.transform.position = position;
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = transform.localScale * 2f;
        this.transform.GetComponent<Collider>().enabled = true;
    }

    public void WobbleEffect()
    {
        this.transform.DOShakeScale(0.3f, 0.2f, 10, 90, false);
    }
}
