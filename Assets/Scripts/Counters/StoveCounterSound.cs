using System;
using UnityEngine;

namespace KitchenKrapper
{
    [RequireComponent(typeof(AudioSource))]
    public class StoveCounterSound : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;
        private AudioSource audioSource;
        private bool shouldPlayWarningSound;
        private float warningSoundTimer;
        private const float WarningSoundTimerMax = 0.2f;
        private const float BurnShowProgress = 0.5f;

        private void Awake()
        {
            // TODO: Rework this to use the Audio Manager
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (stoveCounter != null)
            {
                stoveCounter.OnStoveCounterStateChange += HandleStoveCounterStateChange;
                stoveCounter.OnProgressChanged += HandleProgressChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (stoveCounter != null)
            {
                stoveCounter.OnStoveCounterStateChange -= HandleStoveCounterStateChange;
                stoveCounter.OnProgressChanged -= HandleProgressChanged;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void HandleProgressChanged(object sender, IHasProgress.ProgressChangedEventArgs e)
        {
            shouldPlayWarningSound = stoveCounter.IsDone() && e.progressNormalized >= BurnShowProgress;
        }

        private void HandleStoveCounterStateChange(object sender, StoveCounter.StoveCounterStateChangeEventArgs e)
        {
            switch (e.stoveCounterState)
            {
                case StoveCounter.StoveCounterState.Empty:
                    audioSource.Stop();
                    break;
                case StoveCounter.StoveCounterState.Cooking:
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
                    warningSoundTimer = WarningSoundTimerMax;
                    LevelManager.Instance.PlayWarningSound();
                }
            }
        }
    }
}
