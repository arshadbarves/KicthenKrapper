using System;
using Unity.Netcode;
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

    public void SetStationParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetStationParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStationParentServerRpc(NetworkObjectReference stationNetworkObjectReference)
    {
        SetStationParentClientRpc(stationNetworkObjectReference);
    }

    [ClientRpc]
    private void SetStationParentClientRpc(NetworkObjectReference stationParentNetworkObjectReference)
    {
        stationParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
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
        // set the size to small
        transform.localScale = transform.localScale * 0.5f;

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }

    public void RemoveStationParent(Vector3 position)
    {
        RemoveStationParentServerRpc(position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveStationParentServerRpc(Vector3 position)
    {
        RemoveStationParentClientRpc(position);
    }

    [ClientRpc]
    private void RemoveStationParentClientRpc(Vector3 position)
    {
        if (kitchenObjectParent != null)
        {
            kitchenObjectParent.RemoveKitchenObject();
        }

        kitchenObjectParent = null;
        followTransform.SetTargetTransform(null);
        print(followTransform);
        transform.position = position;
        transform.rotation = Quaternion.identity;
        transform.localScale = transform.localScale * 2f;
    }

    public Mesh GetMesh()
    {
        // Filter children Mesh by Layer
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.gameObject.layer == LayerMask.NameToLayer("GhostMesh"))
            {
                return meshFilter.mesh;
            }
        }

        return null;
    }
}
