using System;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnVisual;
    [SerializeField] private GameObject cookingParticleVisual;

    private void Start()
    {
        stoveCounter.OnStoveCounterStateChange += StoveCounter_OnStoveCounterStateChange;
    }

    private void StoveCounter_OnStoveCounterStateChange(object sender, StoveCounter.StoveCounterStateChangeEventArgs e)
    {
        switch (e.stoveCounterState)
        {
            case StoveCounter.StoveCounterState.Empty:
                stoveOnVisual.SetActive(false);
                cookingParticleVisual.SetActive(false);
                break;
            case StoveCounter.StoveCounterState.Cooking:
                stoveOnVisual.SetActive(true);
                cookingParticleVisual.SetActive(true);
                break;
            case StoveCounter.StoveCounterState.Done:
                stoveOnVisual.SetActive(true);
                cookingParticleVisual.SetActive(true);
                break;
            case StoveCounter.StoveCounterState.Burned:
                stoveOnVisual.SetActive(false);
                cookingParticleVisual.SetActive(false);
                break;
            default:
                break;
        }
    }
}