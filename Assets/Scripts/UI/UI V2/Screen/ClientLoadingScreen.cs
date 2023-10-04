using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class ClientLoadingScreen : Screen
    {
        [SerializeField] private float delayBeforeFadeOut = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.1f;
        [SerializeField] private LoadingProgressManager loadingProgressManager;
        [SerializeField] private bool showLoadingScreenOnStart = true;
        [SerializeField] private LoadingTipsSO loadingTips;

        const string LevelLoadingProgressBar = "level-loading__progress-bar";
        const string LevelLoadingTipLabel = "level-loading__tip-label";

        private ProgressBar progressBar;
        private Label loadingTipLabel;

        // time to show radial progress bar
        const float lerpTime = 1f;

        private bool loadingScreenRunning;
        private Coroutine fadeOutCoroutine;

        protected override void Awake()
        {
            base.Awake();
            SetupLoadingScreen();
        }

        private void SetupLoadingScreen()
        {
            progressBar = root.Q<ProgressBar>(LevelLoadingProgressBar);
            loadingTipLabel = root.Q<Label>(LevelLoadingTipLabel);
        }

        private void Update()
        {
            if (loadingScreenRunning)
            {
                progressBar.value = Mathf.Lerp(progressBar.value, loadingProgressManager.LocalProgress, lerpTime * Time.deltaTime);
                progressBar.title = loadingProgressManager.LocalProgress.ToString("P0");
            }
        }

        public void StopLoadingScreen()
        {
            if (loadingScreenRunning)
            {
                if (fadeOutCoroutine != null)
                {
                    StopCoroutine(fadeOutCoroutine);
                }
                fadeOutCoroutine = StartCoroutine(FadeOutCoroutine());
            }
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            loadingScreenRunning = true;
            loadingTipLabel.text = "Tip: " + loadingTips.GetRandomTip();
            UpdateLoadingScreen();
        }

        public override void HideScreen()
        {
            base.HideScreen();
            loadingScreenRunning = false;
        }

        public void UpdateLoadingScreen()
        {
            if (loadingScreenRunning)
            {
                if (fadeOutCoroutine != null)
                {
                    StopCoroutine(fadeOutCoroutine);
                }
            }
        }

        private IEnumerator FadeOutCoroutine()
        {
            yield return new WaitForSeconds(delayBeforeFadeOut);
            loadingScreenRunning = false;

            float currentTime = 0;
            while (currentTime < fadeOutDuration)
            {
                screen.style.opacity = Mathf.Lerp(1, 0, currentTime / fadeOutDuration);
                yield return null;
                currentTime += Time.deltaTime;
            }
            // Wait for the last frame to finish
            yield return null;
            Invoke("HideScreen", 0.5f);
        }
    }
}
