using TMPro;
using UnityEngine;
using BrunoMikoski.AnimationSequencer;

namespace KitchenKrapper
{
    public class MessageUI : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI messageText;
        private AnimationSequencerController animationSequencerController;

        private void Start()
        {
            animationSequencerController = GetComponent<AnimationSequencerController>();

            Hide();
        }

        public void ShowMessage(string message)
        {
            messageText.text = message;
            Show();
        }

        private void Show()
        {
            // Stop all animations and reset them
            animationSequencerController.Kill();
            animationSequencerController.Play();
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}