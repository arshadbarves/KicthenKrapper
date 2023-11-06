using UnityEngine;
using Unity.Netcode;

namespace KitchenKrapper
{
    // Singleton
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        Debug.LogError($"[Singleton] No instance of {typeof(T)} found in the scene.");
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"[Singleton] An instance of {typeof(T)} already exists. Destroying this instance.");
                Destroy(this);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }

    // NetworkSingleton
    public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        Debug.LogError($"[NetworkSingleton] No instance of {typeof(T)} found in the scene.");
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"[NetworkSingleton] An instance of {typeof(T)} already exists. Destroying this instance.");
                Destroy(this);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        public override void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
