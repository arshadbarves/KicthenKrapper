using Player;
using UnityEngine;

namespace KitchenKrapper
{
    public class PlayerSounds : MonoBehaviour
    {
        private PlayerController player;
        private float footStepTimer = 0f;
        private float footStepTimerMax = 0.1f;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        private void Update()
        {
            HandleFootStepSound();
        }

        private void HandleFootStepSound()
        {
            footStepTimer -= Time.deltaTime;

            if (footStepTimer <= 0f)
            {
                footStepTimer = footStepTimerMax;

                if (player.IsMoving())
                {
                    // LevelManager.Instance.PlayFootstepSound(player.transform.position);
                }
            }
        }
    }
}