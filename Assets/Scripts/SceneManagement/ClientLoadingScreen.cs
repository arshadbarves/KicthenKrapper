using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;


/// <summary>
/// This script handles the use of a loading screen with a progress bar and the name of the loaded scene shown. It
/// must be started and stopped from outside this script. It also allows updating the loading screen when a new
/// loading operation starts before the loading screen is stopped.
/// </summary>
public class ClientLoadingScreen : MonoBehaviour
{
    [SerializeField]
    CanvasGroup m_CanvasGroup;

    [SerializeField]
    float m_DelayBeforeFadeOut = 0.5f;

    [SerializeField]
    float m_FadeOutDuration = 0.1f;

    [SerializeField]
    Slider m_ProgressBar;

    [SerializeField]
    protected LoadingProgressManager m_LoadingProgressManager;

    [SerializeField] private bool m_showLoadingScreenOnStart = true;

    [SerializeField] private AnimationSequencerController m_animationSequencerController;

    bool m_LoadingScreenRunning;

    Coroutine m_FadeOutCoroutine;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        SetCanvasVisibility(false);
    }

    void Update()
    {
        if (m_LoadingScreenRunning)
        {
            m_ProgressBar.value = m_LoadingProgressManager.LocalProgress;
        }
    }

    public void StopLoadingScreen()
    {
        if (m_LoadingScreenRunning)
        {
            if (m_FadeOutCoroutine != null)
            {
                StopCoroutine(m_FadeOutCoroutine);
            }
            m_FadeOutCoroutine = StartCoroutine(FadeOutCoroutine());
        }
    }

    public void StartLoadingScreen()
    {
        SetCanvasVisibility(true);
        m_animationSequencerController.Kill();
        m_animationSequencerController.Play();
        m_LoadingScreenRunning = true;
        UpdateLoadingScreen();
    }

    public void UpdateLoadingScreen()
    {
        if (m_LoadingScreenRunning)
        {
            if (m_FadeOutCoroutine != null)
            {
                StopCoroutine(m_FadeOutCoroutine);
            }
        }
    }

    void SetCanvasVisibility(bool visible)
    {
        m_CanvasGroup.alpha = visible ? 1 : 0;
        m_CanvasGroup.blocksRaycasts = visible;
    }

    IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(m_DelayBeforeFadeOut);
        m_LoadingScreenRunning = false;

        float currentTime = 0;
        while (currentTime < m_FadeOutDuration)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(1, 0, currentTime / m_FadeOutDuration);
            yield return null;
            currentTime += Time.deltaTime;
        }

        SetCanvasVisibility(false);
    }
}
