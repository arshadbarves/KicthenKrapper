using BrunoMikoski.AnimationSequencer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinCodeUI : MonoBehaviour
{
    [Header("Join Code Input")]
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private MessageUI messagePanel;

    private AnimationSequencerController animationSequencerController;

    private void Awake()
    {
        joinButton.onClick.AddListener(JoinButton_OnClick);
        closeButton.onClick.AddListener(CloseButton_OnClick);
        animationSequencerController = GetComponent<AnimationSequencerController>();

        Hide();
    }

    private void CloseButton_OnClick()
    {
        Hide();
    }

    private void JoinButton_OnClick()
    {
        if (string.IsNullOrEmpty(joinCodeInput.text))
        {
            messagePanel.ShowMessage("Please enter a kitchen code");
            return;
        }

        KitchenGameLobby.Instance.JoinWithCode(joinCodeInput.text);
    }

    public void Show()
    {
        // Stop all animations and reset them
        animationSequencerController.Kill();
        animationSequencerController.Play();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
