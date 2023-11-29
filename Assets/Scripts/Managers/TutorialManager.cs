using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using BrunoMikoski.AnimationSequencer;

namespace KitchenKrapper
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private GameObject tutorialOverUI;
        [SerializeField] private GameObject tutorialCanvas;
        [SerializeField] private LevelManager gameManager;
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float padding = 10f;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private Button altInteractButton;
        [SerializeField] private Button interactButton;
        [SerializeField] private GameControllerUI gameController;
        [SerializeField] private TutorialStep[] tutorialSteps;

        private int currentStepIndex;
        private GameObject indicator;
        private bool tutorialCompleted;

        [System.Serializable]
        public struct TutorialStep
        {
            public string text;
            public Transform objectPosition;
            public bool isAltInteract;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            indicator = Instantiate(indicatorPrefab, Vector3.zero, Quaternion.identity);
            indicator.SetActive(false);
        }

        private void Start()
        {
            tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
            if (tutorialCompleted)
            {
                ShowCongratsMessage();
                return;
            }

            currentStepIndex = 0;
            CompleteTutorialStep(currentStepIndex);
            tutorialOverUI.gameObject.SetActive(false);

            EOSKitchenGameMultiplayer.Instance.StartHost();

            StartCoroutine(ShowGameController());
        }

        private void SetTutorialStep(int index)
        {
            tutorialSteps[index].objectPosition.GetComponent<BaseStation>().SetTutorialStepIndex(index);
        }

        public void ShowTutorialStep(TutorialStep step)
        {
            ShowIndicator(step.objectPosition);
            tutorialText.text = step.text;
            AdjustBackgroundSize();
        }

        IEnumerator ShowGameController()
        {
            yield return new WaitForSeconds(0.5f);
            gameController.Show();
        }

        public void CompleteTutorialStep(int index)
        {
            if (index != currentStepIndex)
            {
                return;
            }

            indicator.SetActive(false);

            if (currentStepIndex == tutorialSteps.Length - 1)
            {
                ShowCongratsMessage();
                return;
            }

            currentStepIndex++;
            SetTutorialStep(currentStepIndex);
            ShowTutorialStep(tutorialSteps[currentStepIndex]);

            if (tutorialSteps[currentStepIndex].isAltInteract)
            {
                altInteractButton.GetComponent<AnimationSequencerController>().Play();
            }
            else
            {
                interactButton.GetComponent<AnimationSequencerController>().Play();
            }
        }

        public void ShowTutorialStep(TutorialSteps step, string text)
        {
            ShowIndicator(step.GetObjectPosition());
            tutorialText.text = text;
            AdjustBackgroundSize();
        }

        private void ShowIndicator(Transform position)
        {
            indicator.transform.position = position.position + offset;
            indicator.transform.SetParent(position);
            indicator.SetActive(true);
        }

        private void ShowCongratsMessage()
        {
            NetworkManager.Singleton.Shutdown();
            gameController.Hide();
            tutorialOverUI.gameObject.SetActive(true);
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            tutorialCompleted = true;
        }

        private void AdjustBackgroundSize()
        {
            float newWidth = tutorialText.preferredWidth + padding;
            float newHeight = tutorialText.preferredHeight + padding;
            backgroundImage.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
    }
}
