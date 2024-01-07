using UnityEditor;
using UnityEngine;

public class MovePsgToEditor : EditorWindow
{
    [MenuItem("MyTools/Move Folder To Editor")]
    private static void MoveFolderToEditor()
    {
        MoveFolder("Assets/PSG", "Assets/Editor/PSG");
    }

    [MenuItem("MyTools/Move Folder From Editor")]
    private static void MoveFolderFromEditor()
    {
        MoveFolder("Assets/Editor/PSG", "Assets/PSG");
    }
    
    [MenuItem("MyTools/Fix PSG")]
    private static void FixPsg()
    {
        // Toggles the PSG folder between Editor and non-Editor
        if (AssetDatabase.IsValidFolder("Assets/Editor/PSG"))
        {
            MoveFolderFromEditor();
        }
        else
        {
            MoveFolderToEditor();
        }
    }

    private static void MoveFolder(string sourceFolder, string destinationFolder)
    {
        // Check if the source folder exists
        if (AssetDatabase.IsValidFolder(sourceFolder))
        {
            // Move the folder
            var result = AssetDatabase.MoveAsset(sourceFolder, destinationFolder);
            if (!string.IsNullOrEmpty(result))
            {
                Debug.LogError($"Error moving folder from {sourceFolder} to {destinationFolder}: {result}");
                return;
            }

            // Refresh the Asset Database
            AssetDatabase.Refresh();
            Debug.Log($"Folder moved from {sourceFolder} to {destinationFolder}");
        }
        else
        {
            Debug.LogError($"Source folder {sourceFolder} not found.");
        }
    }
}