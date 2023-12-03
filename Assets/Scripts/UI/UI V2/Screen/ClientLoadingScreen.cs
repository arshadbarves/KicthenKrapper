using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace KitchenKrapper
{
    public class ClientLoadingScreen : BaseScreen
    {
        [SerializeField] private float delayBeforeFadeOut = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.1f;
        [SerializeField] private LoadingProgressManager loadingProgressManager;
        [SerializeField] private bool showLoadingScreenOnStart = true;
        [SerializeField] private LoadingTipsSO loadingTips;

        const string LEVEL_LOADING_PROGRESS_BAR = "level-loading__progress-bar";
        const string LEVEL_LOADING_TIP_LABEL_NAME = "level-loading__tip-label";

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
            progressBar = root.Q<ProgressBar>(LEVEL_LOADING_PROGRESS_BAR);
            loadingTipLabel = root.Q<Label>(LEVEL_LOADING_TIP_LABEL_NAME);
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

        public override void Show()
        {
            base.Show();
            loadingScreenRunning = true;
            loadingTipLabel.text = "Tip: " + loadingTips.GetRandomTip();
            UpdateLoadingScreen();
        }

        public override void Hide()
        {
            base.Hide();
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

        private void HideScreen()
        {
            MainMenuUIManager.Instance.HideLoadingScreen();
        }
    }
}
