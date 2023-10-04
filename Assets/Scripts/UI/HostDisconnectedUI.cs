using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;

namespace KitchenKrapper
{
    public class HostDisconnectedUI : MonoBehaviour
    {
        [SerializeField] private Button exitButton;
        [SerializeField] private GameObject floatingJoystick;
        private AnimationSequencerController animationSequencerController;

        private void Start()
        {
            exitButton.onClick.AddListener(ExitButton_OnClick);
            animationSequencerController = GetComponent<AnimationSequencerController>();
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            Hide();
        }

        private void OnDestroy()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                Show();
            }
        }

        private void ExitButton_OnClick()
        {
            LoadMainMenuScene();
        }

        private void Show()
        {
            floatingJoystick.SetActive(false);
            KillAndPlayAnimation();
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void KillAndPlayAnimation()
        {
            animationSequencerController.Kill();
            animationSequencerController.Play();
        }

        private void LoadMainMenuScene()
        {
            SceneLoaderWrapper.Instance.LoadScene(SceneType.MainMenu.ToString(), false);
        }
    }
}