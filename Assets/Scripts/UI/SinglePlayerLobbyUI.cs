using UnityEngine;

public class SinglePlayerLobbyUI : MonoBehaviour
{
    private void Start()
    {
        if (GameDataSource.playMultiplayer)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
