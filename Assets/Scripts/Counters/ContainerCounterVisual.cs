using System;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private const string INTERACT_TRIGGER = "Interact";
    [SerializeField] private ContainerCounter containerCounter;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        containerCounter.OnContainerCounterInteracted += ContainerCounter_OnContainerCounterInteracted;
    }

    private void ContainerCounter_OnContainerCounterInteracted(object sender, EventArgs e)
    {
        animator.SetTrigger(INTERACT_TRIGGER);
    }
}
