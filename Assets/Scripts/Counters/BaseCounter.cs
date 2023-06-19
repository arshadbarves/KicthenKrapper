using System;
using Unity.Netcode;
using UnityEngine;

public class BaseStation : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedOnCounter;

    private int tutorialStepIndex;

    public int GetTutorialStepIndex()
    {
        return tutorialStepIndex;
    }

    public void SetTutorialStepIndex(int tutorialStepIndex)
    {
        this.tutorialStepIndex = tutorialStepIndex;
    }

    public static void ResetStaticData()
    {
        OnAnyObjectPlacedOnCounter = null;
    }

    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public virtual void Interact(Player player)
    {
        Debug.LogException(new System.Exception("Interact() not implemented in " + this.GetType().Name));
    }

    public virtual void InteractAlternate(Player player)
    {
        // Debug.LogException(new System.Exception("InteractAlternate() not implemented in " + this.GetType().Name));
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

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.CompleteTutorialStep(GetTutorialStepIndex());
        }
    }
}
