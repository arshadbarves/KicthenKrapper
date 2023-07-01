using System;
using Unity.Netcode;
using UnityEngine;

public class BaseStation : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedOnCounter;

    [SerializeField] private Transform counterTopPoint;

    private int tutorialStepIndex;
    private KitchenObject kitchenObject;

    public int TutorialStepIndex => tutorialStepIndex;

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
}
