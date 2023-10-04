using UnityEngine;

namespace KitchenKrapper
{
    public class SinglePlayerLobbyUI : MonoBehaviour
    {
        private void Start()
        {
            if (GameDataSource.PlayMultiplayer)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }
}