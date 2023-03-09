using System.Collections;
using UnityEngine;

public class AICharacter : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform deliveryPoint;
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float deliveryWaitTime = 2f;
    public LayerMask aiCharacterLayer;

    private Vector3 direction;
    private bool waitingForDelivery = false;
    private float deliveryCounter = 0f;
    private bool returningToSpawn = false;

    private void Start()
    {
        transform.position = spawnPoint.position;
        direction = (deliveryPoint.position - transform.position).normalized;
    }

    private void Update()
    {
        if (waitingForDelivery)
        {
            deliveryCounter -= Time.deltaTime;
            if (deliveryCounter <= 0f)
            {
                waitingForDelivery = false;
                returningToSpawn = true;
                direction = (spawnPoint.position - transform.position).normalized;
            }
        }
        else if (returningToSpawn)
        {
            float distance = Vector3.Distance(transform.position, spawnPoint.position);
            if (distance < 0.5f)
            {
                Destroy(gameObject);
            }
            else
            {
                float easedSpeed = EaseOutCubic(0, speed, Time.deltaTime);
                transform.position = transform.position + direction * easedSpeed;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            float distance = Vector3.Distance(transform.position, deliveryPoint.position);
            if (distance < 0.5f)
            {
                waitingForDelivery = true;
                deliveryCounter = deliveryWaitTime;
            }
            else
            {
                if (Physics.OverlapSphere(transform.position + direction, 0.5f, aiCharacterLayer).Length > 0)
                {
                    return;
                }

                float easedSpeed = EaseOutCubic(0, speed, Time.deltaTime);
                transform.position = transform.position + direction * easedSpeed;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    public void TriggerDelivery()
    {
        deliveryCounter = 0f;
    }

    float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }
}
