using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
