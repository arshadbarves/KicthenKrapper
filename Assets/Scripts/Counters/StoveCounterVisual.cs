using System;
using UnityEngine;

namespace KitchenKrapper
{
    public class StoveCounterVisual : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;
        [SerializeField] private GameObject stoveOnVisual;
        [SerializeField] private GameObject cookingParticleVisual;

        private void Start()
        {
            // Subscribe to the event in Awake for proper initialization
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            // Unsubscribe from the event to prevent memory leaks
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            stoveCounter.OnStoveCounterStateChange += HandleStoveCounterStateChange;
        }

        private void UnsubscribeFromEvents()
        {
            stoveCounter.OnStoveCounterStateChange -= HandleStoveCounterStateChange;
        }

        private void HandleStoveCounterStateChange(object sender, StoveCounter.StoveCounterStateChangeEventArgs e)
        {
            // Determine the visibility of visual elements based on stove counter state
            bool stoveOnVisible = e.stoveCounterState == StoveCounter.StoveCounterState.Cooking ||
                                  e.stoveCounterState == StoveCounter.StoveCounterState.Done;

            // Set the visibility of visual elements
            stoveOnVisual.SetActive(stoveOnVisible);
            cookingParticleVisual.SetActive(stoveOnVisible);
        }
    }
}
