using System;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string COUNTDOWN_ANIMATION_TRIGGER = "Countdown";

    [SerializeField] private TextMeshProUGUI countdownText;

    private Animator animator;
    private int previousCountdown = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        LevelManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        bool isCountdownToStart = LevelManager.Instance.IsCountdownToStart();
        if (isCountdownToStart)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        int countdown = Mathf.CeilToInt(LevelManager.Instance.GetCountdownToStartTimer());
        UpdateCountdownText(countdown);

        if (countdown != previousCountdown)
        {
            previousCountdown = countdown;
            TriggerCountdownAnimation();
            PlayCountdownSound();
        }
    }

    private void UpdateCountdownText(int countdown)
    {
        countdownText.text = countdown.ToString();
    }

    private void TriggerCountdownAnimation()
    {
        animator.SetTrigger(COUNTDOWN_ANIMATION_TRIGGER);
    }

    private void PlayCountdownSound()
    {
        LevelManager.Instance.PlayCountdownSound();
    }
}
