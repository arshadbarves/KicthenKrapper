using System;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource audioSource;
    private float warningSoundTimer;
    private bool shouldPlayWarningSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStoveCounterStateChange += StoveCounter_OnStoveCounterStateChange;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.ProgressChangedEventArgs e)
    {
        float burnShowProgress = 0.5f;
        shouldPlayWarningSound = stoveCounter.IsDone() && e.progressNormalized >= burnShowProgress;
    }

    private void StoveCounter_OnStoveCounterStateChange(object sender, StoveCounter.StoveCounterStateChangeEventArgs e)
    {
        switch (e.stoveCounterState)
        {
            case StoveCounter.StoveCounterState.Empty:
                audioSource.Stop();
                break;
            case StoveCounter.StoveCounterState.Cooking:
                audioSource.Play();
                break;
            case StoveCounter.StoveCounterState.Done:
                audioSource.Play();
                break;
            case StoveCounter.StoveCounterState.Burned:
                audioSource.Stop();
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (shouldPlayWarningSound)
        {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer <= 0f)
            {
                float warningSoundTimerMax = 0.2f;
                warningSoundTimer = warningSoundTimerMax;

                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            }
        }
    }
}
