using Unity.Netcode;
using UnityEngine;

namespace KitchenKrapper
{
    public class PlayerAnimator : NetworkBehaviour
    {
        private const string IS_WALKING = "IsWalking";

        [SerializeField] private PlayerController player;
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!IsOwner) return;

            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            animator.SetBool(IS_WALKING, player.IsWalking());
        }
    }
}
