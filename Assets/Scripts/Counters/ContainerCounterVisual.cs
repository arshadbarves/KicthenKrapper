using System;
using UnityEngine;

namespace KitchenKrapper
{
    public class ContainerCounterVisual : MonoBehaviour
    {
        private const string INTERACT_TRIGGER = "Interact";

        [SerializeField] private ContainerCounter containerCounter;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            containerCounter.OnContainerCounterInteracted += HandleContainerCounterInteracted;
        }

        private void OnDisable()
        {
            containerCounter.OnContainerCounterInteracted -= HandleContainerCounterInteracted;
        }

        private void HandleContainerCounterInteracted(object sender, EventArgs e)
        {
            TriggerInteractAnimation();
        }

        private void TriggerInteractAnimation()
        {
            animator.SetTrigger(INTERACT_TRIGGER);
        }
    }
}