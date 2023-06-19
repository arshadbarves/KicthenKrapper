using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;
using Unity.Netcode;

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
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
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
        // Check if the tutorial is completed
        tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        if (tutorialCompleted)
        {
            // Show the congratulatory message
            ShowCongratsMessage();
            return;
        }

        currentStepIndex = 0;
        // Show the first tutorial step
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
        // Show Indicator on top of the object
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

        // Hide the indicator
        indicator.SetActive(false);

        // Check if the tutorial is completed
        if (currentStepIndex == tutorialSteps.Length - 1)
        {
            ShowCongratsMessage();
            return;
        }

        // Show the next tutorial step
        currentStepIndex++;
        SetTutorialStep(currentStepIndex);
        ShowTutorialStep(tutorialSteps[currentStepIndex]);

        // Show the interact button
        if (tutorialSteps[currentStepIndex].isAltInteract)
        {
            altInteractButton.GetComponent<AnimationSequencerController>().Play();
            interactButton.GetComponent<AnimationSequencerController>().ResetComplete();
        }
        else
        {
            altInteractButton.GetComponent<AnimationSequencerController>().ResetComplete();
            interactButton.GetComponent<AnimationSequencerController>().Play();
        }
    }

    public void ShowTutorialStep(TutorialSteps step, string text)
    {
        // Show Indicator on top of the object
        ShowIndicator(step.GetObjectPosition());
        tutorialText.text = text;

        AdjustBackgroundSize();
    }

    private void ShowIndicator(Transform position)
    {
        // Set the indicator's position
        indicator.transform.position = position.position + offset;
        indicator.transform.SetParent(position);

        // Show/hide the indicator and its children
        indicator.SetActive(true);
    }

    private void ShowCongratsMessage()
    {
        NetworkManager.Singleton.Shutdown();
        print("Tutorial Completed");
        gameController.Hide();
        // Show the congratulatory message
        tutorialOverUI.gameObject.SetActive(true);
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        tutorialCompleted = true;
    }

    private void AdjustBackgroundSize()
    {
        // Calculate the preferred width and height of the text length and padding
        float newWidth = tutorialText.preferredWidth + padding;
        float newHeight = tutorialText.preferredHeight + padding;

        // Set the size of the background image
        backgroundImage.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
