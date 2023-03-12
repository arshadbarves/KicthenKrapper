using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;

public class HostDisconnectedUI : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject floatingJoystick;
    private AnimationSequencerController animationSequencerController;

    private void Start()
    {
        exitButton.onClick.AddListener(ExitButton_OnClick);
        animationSequencerController = GetComponent<AnimationSequencerController>();
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        Hide();
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
    }

    private void Singleton_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            Show();
        }
    }

    private void ExitButton_OnClick()
    {
        SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), false);
    }

    private void Show()
    {
        floatingJoystick.gameObject.SetActive(false);
        animationSequencerController.Kill();
        animationSequencerController.Play();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
