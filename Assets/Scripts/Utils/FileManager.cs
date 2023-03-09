// using System.IO;
// using UnityEngine;

// public static class FileManager
// {
//     public static string LoadFileContents(string filePath)
//     {
//         string fileContents = string.Empty;

//         try
//         {
//             fileContents = File.ReadAllText(filePath);
//         }
//         catch (FileNotFoundException e)
//         {
//             Debug.LogError($"File {filePath} not found. Error: {e.Message}");
//         }
//         catch (IOException e)
//         {
//             Debug.LogError($"Error reading file {filePath}. Error: {e.Message}");
//         }

//         return fileContents;
//     }
// }

// public static class ConfigurationParser
// {
//     public static ConfigurationData ParseConfigurationData(string configurationDataJson)
//     {
//         ConfigurationData configData = null;

//         try
//         {
//             configData = JsonUtility.FromJson<ConfigurationData>(configurationDataJson);
//         }
//         catch (System.ArgumentException e)
//         {
//             Debug.LogError($"Error parsing configuration data. Error: {e.Message}");
//         }

//         return configData;
//     }
// }
