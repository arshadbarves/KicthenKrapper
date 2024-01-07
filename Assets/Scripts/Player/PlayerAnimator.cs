using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerAnimator : NetworkBehaviour
    {
        private const string IsMoving = "IsMoving";

        [SerializeField] private PlayerController player;
        private Animator _animator;
        private static readonly int Moving = Animator.StringToHash(IsMoving);

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!IsOwner) return;
            Debug.Log("Updating animator");
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            _animator.SetBool(Moving, player.IsMoving());
        }
    }
}
