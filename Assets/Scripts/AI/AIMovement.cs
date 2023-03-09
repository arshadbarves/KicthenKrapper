using UnityEngine;

public class AIMovement : MonoBehaviour
{
    private const string IS_WALKING = "IsMoving";
    private Animator animator;

    public float speed = 1f; // movement speed of AI character
    public float arrivalDistance = 0.5f; // distance at which AI character is considered to have arrived at its destination

    private Vector3 target; // target destination for AI character
    private bool destroyOnArrival = false; // flag to determine if AI character should be destroyed upon arrival

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // check if there is a target destination set
        if (target != Vector3.zero)
        {
            animator.SetBool(IS_WALKING, true);
            // calculate distance to target destination
            float distance = Vector3.Distance(transform.position, target);

            // check if AI character has arrived at its destination
            if (distance < arrivalDistance)
            {
                // check if AI character should be destroyed upon arrival
                if (destroyOnArrival)
                {
                    Destroy(gameObject);
                }
                else
                {
                    // reset target destination
                    target = Vector3.zero;
                }
            }
            else
            {
                // move AI character towards target destination
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                // rotate AI character towards target destination
                transform.rotation = Quaternion.LookRotation(target - transform.position);
            }
        }
        else
        {
            animator.SetBool(IS_WALKING, false);
        }
    }

    // method to set target destination for AI character and destroyOnArrival flag, if another AI character is already at the spawn point it will wait for a while before moving to the delivery point at a distance of waitDistance, if the spawn point is free it will move to the delivery point immediately
    public void MoveToPoint(Vector3 target, bool destroyOnArrival = false)
    {
        this.target = target;
        this.destroyOnArrival = destroyOnArrival;
    }
}
