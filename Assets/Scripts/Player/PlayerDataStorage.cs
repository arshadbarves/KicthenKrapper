using System;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using UnityEngine;

namespace KitchenKrapper
{

    public class PlayerDataStorage : MonoBehaviour
    {
        public static event Action PlayerDataCreated;
        public static PlayerDataStorage Instance { get; private set; }

        private const string FILE_NAME = "PlayerData";
        private EOSPlayerDataStorageManager PlayerDataStorageManager;

        private void Awake()
        {
            InitializeSingleton();
            InitializePlayerDataStorageManager();
        }

        private void OnDestroy()
        {
            CleanupPlayerDataStorageManager();
        }

        private void InitializeSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void InitializePlayerDataStorageManager()
        {
            PlayerDataStorageManager = EOSManager.Instance.GetOrCreateManager<EOSPlayerDataStorageManager>();
        }

        private void CleanupPlayerDataStorageManager()
        {
            EOSManager.Instance.RemoveManager<EOSPlayerDataStorageManager>();
        }

        public void CreatePlayerData(PlayerGameData playerData)
        {
            string newFileContents = playerData.ToJson();

            print("[PlayerDataStorage]: Creating File " + newFileContents);

            PlayerDataStorageManager.AddFile(FILE_NAME, newFileContents, () =>
            {
                print("[PlayerDataStorage]: Player data created");
                PlayerDataCreated?.Invoke();
            });
        }

        public void GetPlayerData(Action<string> callback)
        {
            string fileContent = PlayerDataStorageManager.GetCachedFileContent(FILE_NAME);

            if (fileContent == null)
            {
                Debug.Log("[PlayerDataStorage]: Player data not found, downloading from cloud");
                PlayerDataStorageManager.StartFileDataDownload(FILE_NAME, () =>
                {
                    string newFileContent = PlayerDataStorageManager.GetCachedFileContent(FILE_NAME);
                    if (string.IsNullOrEmpty(newFileContent))
                    {
                        Debug.Log("[PlayerDataStorage]: Player data not found on cloud");
                        callback(null);
                    }
                    else
                    {
                        Debug.Log("[PlayerDataStorage]: Player data found" + (newFileContent.Length == 0 ? ", but empty" : " on cloud, returning"));
                        callback(newFileContent);
                    }
                });
            }
            else
            {
                Debug.Log("[PlayerDataStorage]: Player data found" + (string.IsNullOrEmpty(fileContent) ? ", but empty" : ", returning"));
                callback(fileContent);
            }
        }

        public void SetPlayerData(string playerData)
        {
            PlayerDataStorageManager.AddFile(FILE_NAME, playerData, () => Debug.Log("[PlayerDataStorage]: Player data saved"));
        }

        public void DeletePlayerData()
        {
            PlayerDataStorageManager.DeleteFile(FILE_NAME);
            Debug.Log("[PlayerDataStorage]: Player data deleted");
        }
    }

}