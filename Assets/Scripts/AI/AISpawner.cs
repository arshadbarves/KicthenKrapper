using System;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    public GameObject[] AIPrefab; // prefab of AI character
    public Transform deliveryPoint; // delivery point transform
    public float waitDistance = 2f; // distance for AI character to wait if spawn point is occupied

    private GameObject AI;

    private bool spawnPointOccupied = false; // flag to determine if spawn point is occupied


    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
    }

    private void DeliveryManager_OnRecipeDelivered(object sender, EventArgs e)
    {
        if (AI == null) return;
        // make AI character return to spawn point and destroy it
        AI.GetComponent<AIMovement>().MoveToPoint(transform.position, true);
        spawnPointOccupied = false;
        // spawn a new AI character
        SpawnAICharacter();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
    {
        SpawnAICharacter();
    }

    private void SpawnAICharacter()
    {
        // check if there is already an AI character at the spawn point
        if (AI != null)
        {
            spawnPointOccupied = true;
        }
        else
        {
            spawnPointOccupied = false;
        }
        // check if spawn point is occupied
        if (!spawnPointOccupied)
        {
            int random = UnityEngine.Random.Range(0, AIPrefab.Length);
            // spawn a new AI character
            AI = Instantiate(AIPrefab[random], transform.position, transform.rotation);

            // make AI character move to delivery point
            AI.GetComponent<AIMovement>().MoveToPoint(deliveryPoint.position);
        }
        // else
        // {
        //     // spawn a new AI character
        //     AI = Instantiate(AIPrefab, transform.position, transform.rotation);

        //     // make AI character move to delivery point at a distance of waitDistance
        //     AI.GetComponent<AIMovement>().MoveToPoint(deliveryPoint.position + (transform.position - deliveryPoint.position).normalized * waitDistance);
        // }
    }
}
