using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerLobbyUI : MonoBehaviour
{
    private void Start()
    {
        if (KitchenGameMultiplayer.playMultiplayer)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
