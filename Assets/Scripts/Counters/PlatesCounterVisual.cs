using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;

    private List<GameObject> platesVisualGameObjects;

    private void Awake()
    {
        platesVisualGameObjects = new List<GameObject>();
    }

    private void Start()
    {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
    }

    private void PlatesCounter_OnPlateRemoved(object sender, EventArgs e)
    {
        GameObject lastPlateVisualGameObject = platesVisualGameObjects[platesVisualGameObjects.Count - 1];
        platesVisualGameObjects.Remove(lastPlateVisualGameObject);
        Destroy(lastPlateVisualGameObject);
    }

    private void PlatesCounter_OnPlateSpawned(object sender, EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        float plateVisualHeight = 0.1f;
        plateVisualTransform.localPosition = new Vector3(0f, plateVisualHeight * platesVisualGameObjects.Count, 0f);

        platesVisualGameObjects.Add(plateVisualTransform.gameObject);
    }
}
