using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenKrapper
{
    public class GamePlayClockUI : MonoBehaviour
    {
        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerText;

        private void Update()
        {
            UpdateTimerImage();
            UpdateTimerText();
        }

        private void UpdateTimerImage()
        {
            timerImage.fillAmount = LevelManager.Instance.GetGamePlayingTimer(true);
        }

        private void UpdateTimerText()
        {
            float time = LevelManager.Instance.GetGamePlayingTimer();
            if (time <= 0f)
            {
                return;
            }

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.RoundToInt(time % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}