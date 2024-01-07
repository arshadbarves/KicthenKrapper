using Unity.Netcode;
using UnityEngine;

// Singleton
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<T>();

            if (_instance == null)
            {
                Debug.LogError($"[Singleton] No instance of {typeof(T)} found in the scene.");
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[Singleton] An instance of {typeof(T)} already exists. Destroying this instance.");
            Destroy(this);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

// NetworkSingleton
public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<T>();

            if (_instance == null)
            {
                Debug.LogError($"[NetworkSingleton] No instance of {typeof(T)} found in the scene.");
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[NetworkSingleton] An instance of {typeof(T)} already exists. Destroying this instance.");
            Destroy(this);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}