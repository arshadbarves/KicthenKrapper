using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;
using PlayEveryWare.EpicOnlineServices.Samples;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;

namespace KitchenKrapper
{
    public class LobbyCreateUI : MonoBehaviour
    {
        [Header("Lobbies UI - Create Options")]
        [SerializeField] public int MaxPlayersVal;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_InputField lobbyNameInputField;
        [SerializeField] private Toggle privateLobbyToggle;
        [SerializeField] private MessageUI messagePanel;

        private AnimationSequencerController animationSequencerController;

        private void Awake()
        {
            animationSequencerController = GetComponent<AnimationSequencerController>();
            createLobbyButton.onClick.AddListener(CreateNewLobbyButtonOnClick);
            closeButton.onClick.AddListener(CloseButton_OnClick);

            Hide();
        }

        private void Start()
        {
            // MaxPlayersVal = MultiplayerManager.MAX_PLAYERS;
        }

        private void CloseButton_OnClick()
        {
            Hide();
        }

        public void CreateNewLobbyButtonOnClick()
        {
            if (string.IsNullOrEmpty(lobbyNameInputField.text))
            {
                messagePanel.ShowMessage("Please enter a kitchen name");
                return;
            }


            // LobbyManager.Instance.CreateLobby(lobbyProperties);
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
}