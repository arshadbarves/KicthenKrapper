using UnityEngine;
using UnityEngine.UI;

public class TestDebugNet : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Awake()
    {
        createGameButton.onClick.AddListener(CreateGameButton_OnClick);
        joinGameButton.onClick.AddListener(JoinGameButton_OnClick);
    }

    private void CreateGameButton_OnClick()
    {
        KitchenGameMultiplayer.Instance.StartHost();
        SceneLoaderWrapper.Instance.LoadScene(SceneType.Lobby.ToString(), GameDataSource.Instance.UseNetworkSceneManager());
    }

    private void JoinGameButton_OnClick()
    {
        KitchenGameMultiplayer.Instance.StartClient();
    }
}

