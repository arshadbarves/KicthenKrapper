using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
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

        private void OnEnable()
        {
            SubscribeToPlateEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromPlateEvents();
        }

        private void SubscribeToPlateEvents()
        {
            platesCounter.OnPlateSpawned += HandlePlateSpawned;
            platesCounter.OnPlateRemoved += HandlePlateRemoved;
        }

        private void UnsubscribeFromPlateEvents()
        {
            platesCounter.OnPlateSpawned -= HandlePlateSpawned;
            platesCounter.OnPlateRemoved -= HandlePlateRemoved;
        }

        private void HandlePlateSpawned(object sender, EventArgs e)
        {
            CreatePlateVisual();
        }

        private void HandlePlateRemoved(object sender, EventArgs e)
        {
            RemoveLastPlateVisual();
        }

        private void CreatePlateVisual()
        {
            Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

            float plateVisualHeight = 0.1f;
            plateVisualTransform.localPosition = new Vector3(0f, plateVisualHeight * platesVisualGameObjects.Count, 0f);

            platesVisualGameObjects.Add(plateVisualTransform.gameObject);
        }

        private void RemoveLastPlateVisual()
        {
            if (platesVisualGameObjects.Count > 0)
            {
                GameObject lastPlateVisualGameObject = platesVisualGameObjects[platesVisualGameObjects.Count - 1];
                platesVisualGameObjects.Remove(lastPlateVisualGameObject);
                Destroy(lastPlateVisualGameObject);
            }
        }
    }
}
