using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using BrunoMikoski.AnimationSequencer;

public class ConnectingUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI messageText;
    private AnimationSequencerController animationSequencerController;

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        animationSequencerController = GetComponent<AnimationSequencerController>();

        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if (messageText.text == "")
        {
            messageText.text = "Failed to connect to game";
        }

        Show();
    }

    private void KitchenGameMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        messageText.text = "Connecting to game...";
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {

        // animationSequencerController.Play();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
    }
}
