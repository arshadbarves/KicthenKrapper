using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;
    private float footStepTimer = 0f;
    private float footStepTimerMax = 0.1f;

    private void Awake()
    {
        player = GetComponent<Player>();
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

            if (player.IsWalking())
            {
                // LevelManager.Instance.PlayFootstepSound(player.transform.position);
            }
        }
    }
}
