using System;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string COUNTDOWN_ANIMATION_TRIGGER = "Countdown";

    [SerializeField] private TextMeshProUGUI countdownText;

    private Animator animator;
    private int previuosCountdown = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStart())
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
        int countdown = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
        countdownText.text = countdown.ToString();

        if (countdown != previuosCountdown)
        {
            previuosCountdown = countdown;
            animator.SetTrigger(COUNTDOWN_ANIMATION_TRIGGER);
            SoundManager.Instance.PlayCountdownSound();
        }
    }
}
