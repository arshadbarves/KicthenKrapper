using TMPro;
using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using PlayEveryWare.EpicOnlineServices.Samples;
using Utils.Enums;

namespace KitchenKrapper
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("Ready Button")]
        [SerializeField] private Button readyButton;
        [SerializeField] private Sprite onReadyButtonImage;
        [SerializeField] private Sprite onNotReadyButtonImage;
        [SerializeField] private TextMeshProUGUI readyText;

        [Header("Leave Lobby Button")]
        [SerializeField] private Button leaveLobbyButton;

        [Header("Vote Map Button")]
        [SerializeField] private Button voteMapButton;

        [Header("Lobby Info")]
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        private void Awake()
        {
            readyButton.onClick.AddListener(OnReadyButtonClicked);
            leaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClicked);
            voteMapButton.onClick.AddListener(OnVoteMapButtonClicked);
        }

        private void Start()
        {
            // Set Ready Button to Not Ready
            readyText.text = "Not Ready";
            readyButton.GetComponent<Image>().sprite = onNotReadyButtonImage;

            Lobby lobby = LobbyManager.Instance.GetCurrentLobby();

            if (lobby == null)
            {
                Debug.LogError("Lobby is null");
            }

            // Get Level Name

            // lobbyNameText.text = lobby.Attributes.Find(x => x.Key == EOSKitchenGameLobby.LOBBY_NAME).AsAttribute.Value.AsUtf8;
            lobbyCodeText.text = "Code: " + lobby.Id;
        }

        private void OnVoteMapButtonClicked()
        {
            throw new NotImplementedException();
        }

        private void OnLeaveLobbyButtonClicked()
        {
            MultiplayerManager.Instance.Disconnect();
            LobbyManager.Instance.LeaveLobby();
            SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), false);
        }

        private void OnReadyButtonClicked()
        {
            // if (PlayerState.Instance.IsPlayerReady())
            // {
            //     readyText.text = "Not Ready";
            //     readyButton.GetComponent<Image>().sprite = onNotReadyButtonImage;
            // }
            // else
            // {
            //     readyText.text = "Ready";
            //     readyButton.GetComponent<Image>().sprite = onReadyButtonImage;
            // }

            // Call server RPC to set player ready
            // PlayerState.Instance.OnReadyButtonClicked();
        }
    }
}