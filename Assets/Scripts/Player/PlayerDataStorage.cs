using System;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using UnityEngine;

public class PlayerDataStorage : MonoBehaviour
{
    public static PlayerDataStorage Instance { get; private set; }
    private EOSPlayerDataStorageManager PlayerDataStorageManager;

    private const string FILE_NAME = "PlayerData";
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        PlayerDataStorageManager = EOSManager.Instance.GetOrCreateManager<EOSPlayerDataStorageManager>();
    }

    private void OnDestroy()
    {
        EOSManager.Instance.RemoveManager<EOSPlayerDataStorageManager>();
    }

    public void CreatePlayerData(PlayerDataInventory playerData)
    {
        string newFileContents = JsonUtility.ToJson(playerData, true);

        PlayerDataStorageManager.AddFile(FILE_NAME, newFileContents, () => Debug.Log("[PlayerDataStorage]: Player data created"));
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
                if (newFileContent == null)
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
            Debug.Log("[PlayerDataStorage]: Player data found" + (fileContent.Length == 0 ? ", but empty" : ", returning"));
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
