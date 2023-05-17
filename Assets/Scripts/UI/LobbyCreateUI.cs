using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;
// using PlayEveryWare.EpicOnlineServices.Samples;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace Playcenter.KitchenGame.UI
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
            MaxPlayersVal = EOSKitchenGameMultiplayer.MAX_PLAYERS;
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

            if (EOSKitchenGameLobby.BUCKET_ID.Length == 0)
            {
                Debug.LogError("Tried to create new lobby but missing BucketId!");
                return;
            }

            Lobby lobbyProperties = new Lobby();
            // Bucket Id
            lobbyProperties.BucketId = EOSKitchenGameLobby.BUCKET_ID;

            // Max Players
            lobbyProperties.MaxNumLobbyMembers = uint.Parse(MaxPlayersVal.ToString());

            // Level Attributes
            LobbyAttribute lobbyAttribute = new LobbyAttribute();
            lobbyAttribute.Key = EOSKitchenGameLobby.LOBBY_NAME;
            lobbyAttribute.AsString = lobbyNameInputField.text;
            lobbyAttribute.ValueType = AttributeType.String;
            lobbyAttribute.Visibility = LobbyAttributeVisibility.Public;
            lobbyProperties.Attributes.Add(lobbyAttribute);

            // Lobby visibility
            if (privateLobbyToggle.isOn)
            {
                Debug.Log("Creating private lobby");
                lobbyProperties.LobbyPermissionLevel = LobbyPermissionLevel.Joinviapresence;
            }
            else
            {
                Debug.Log("Creating public lobby");
                lobbyProperties.LobbyPermissionLevel = LobbyPermissionLevel.Publicadvertised;
            }

            // Allow Invites
            lobbyProperties.AllowInvites = true;

            // Presence Enabled
            lobbyProperties.PresenceEnabled = false;

            lobbyProperties.RTCRoomEnabled = false;

            EOSKitchenGameLobby.Instance.CreateLobby(lobbyProperties);
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