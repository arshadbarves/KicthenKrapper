using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] private bool m_isDebugging = false;
    void Awake()
    {
        // Check if the game is in debug mode. If it is, then delete all the player prefs.
        if (m_isDebugging)
        {
            // Clear the Player Prefs.
            PlayerPrefs.DeleteAll();
        }
    }
}
