using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Toggle privateLobbyToggle;
    [SerializeField] private MessageUI messagePanel;

    private AnimationSequencerController animationSequencerController;

    private void Awake()
    {
        animationSequencerController = GetComponent<AnimationSequencerController>();
        createLobbyButton.onClick.AddListener(CreateLobbyButton_OnClick);
        closeButton.onClick.AddListener(CloseButton_OnClick);

        Hide();
    }

    private void CloseButton_OnClick()
    {
        Hide();
    }

    private void CreateLobbyButton_OnClick()
    {
        if (string.IsNullOrEmpty(lobbyNameInputField.text))
        {
            messagePanel.ShowMessage("Please enter a kitchen name");
            return;
        }

        KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, privateLobbyToggle.isOn);
        Hide();
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
