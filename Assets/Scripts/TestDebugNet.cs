using Unity.Netcode;
using UnityEngine;

public class TestDebugNet : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H pressed");
            NetworkManager.Singleton.StartHost();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C pressed");
            NetworkManager.Singleton.StartClient();
        }
    }
}

