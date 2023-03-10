using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

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
    }

    private void OnVoteMapButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void OnLeaveLobbyButtonClicked()
    {
        SceneLoaderWrapper.Instance.LoadScene(SceneType.MultiPlayerMenu.ToString(), false);
    }

    private void OnReadyButtonClicked()
    {
        if (PlayerStateReady.Instance.IsReady())
        {
            readyText.text = "Not Ready";
            readyButton.GetComponent<Image>().sprite = onNotReadyButtonImage;
        }
        else
        {
            readyText.text = "Ready";
            readyButton.GetComponent<Image>().sprite = onReadyButtonImage;
        }

        // Call server RPC to set player ready
        PlayerStateReady.Instance.OnReadyButtonClicked();
    }
}
