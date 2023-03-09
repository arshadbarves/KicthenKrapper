using UnityEngine;
using System.Collections.Generic;

public class CarAI : MonoBehaviour
{
    public List<Transform> targets;
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 5f;

    private int currentTarget = 0;
    private float waitTime = 0f;

    void Start()
    {
        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    private void Update()
    {

        if (waitTime == Mathf.Infinity)
        {
            return;
        }

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }

        Vector3 direction = (targets[currentTarget].position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targets[currentTarget].position);


        // If we are close enough to the target, move to the next one and wait for a random amount of time
        if (distance < 0.5f)
        {
            waitTime = Random.Range(minWaitTime, maxWaitTime);

            currentTarget = (currentTarget + 1) % targets.Count;
        }

        float easedSpeed = EaseOutCubic(0, speed, Time.deltaTime);
        transform.position = transform.position + direction * easedSpeed;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

    }

    float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }
}
