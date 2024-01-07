using System.Collections;
using KitchenKrapper;
using Managers;
using SceneManagement;
using UI.Base;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Screen
{
    public class ClientLoadingScreen : BaseScreen
    {
        [SerializeField] private float delayBeforeFadeOut = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.1f;
        [SerializeField] private LoadingProgressManager loadingProgressManager;
        [SerializeField] private LoadingTipsSO loadingTips;
        private const string LevelLoadingProgressBar = "level-loading__progress-bar";
        private const string LevelLoadingTipLabelName = "level-loading__tip-label";
        private ProgressBar _progressBar;
        private Label _loadingTipLabel;

        // time to lerp the progress bar value
        private const float LerpTime = 0.5f;
        private bool _loadingScreenRunning;
        private Coroutine _fadeOutCoroutine;

        protected override void Awake()
        {
            base.Awake();
            SetupLoadingScreen();
        }

        private void Start()
        {
            loadingProgressManager.onTrackersUpdated += UpdateLoadingScreen;
        }

        private void SetupLoadingScreen()
        {
            _progressBar = Root.Q<ProgressBar>(LevelLoadingProgressBar);
            _loadingTipLabel = Root.Q<Label>(LevelLoadingTipLabelName);
        }

        private void Update()
        {
            if (!_loadingScreenRunning) return;
            _progressBar.value = Mathf.Lerp(_progressBar.value, loadingProgressManager.LocalProgress * 100f,
                 LerpTime);
            _progressBar.title = loadingProgressManager.LocalProgress.ToString("P0");
        }

        public void StopLoadingScreen()
        {
            if (!_loadingScreenRunning) return;
            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
            }

            _fadeOutCoroutine = StartCoroutine(FadeOutCoroutine());
        }

        public override void Show()
        {
            base.Show();
            _loadingScreenRunning = true;
            _loadingTipLabel.text = "Tip: " + loadingTips.GetRandomTip();
            UpdateLoadingScreen();
        }

        public override void Hide()
        {
            base.Hide();
            _loadingScreenRunning = false;
        }

        public void UpdateLoadingScreen()
        {
            if (!_loadingScreenRunning) return;
            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
            }
        }

        private IEnumerator FadeOutCoroutine()
        {
            yield return new WaitForSeconds(delayBeforeFadeOut);
            _loadingScreenRunning = false;
            float currentTime = 0;
            while (currentTime < fadeOutDuration)
            {
                Screen.style.opacity = Mathf.Lerp(1, 0, currentTime / fadeOutDuration);
                yield return null;
                currentTime += Time.deltaTime;
            }

            // Wait for the last frame to finish
            yield return null;
            Invoke(nameof(HideScreen), 0.5f);
        }

        private void HideScreen()
        {
            MainMenuUIManager.Instance.HideLoadingScreen();
        }
    }
}