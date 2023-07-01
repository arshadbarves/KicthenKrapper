using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour, IGenerator
{
    public List<PSGAsset> wallPrefabs;
    public void ClearMeshes()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
    }

    public void Generate(List<Cell> cells, Vector2Int gridSize)
    {
        ClearMeshes();

        foreach (var cell in cells)
        {
            if (cell.cellTag == CellTag.Wall)
            {
                print("wall");
                GameObject wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Count)].prefab, transform);
                wall.transform.position = cell.position;
                wall.transform.rotation = cell.rotation;
            }
        }
    }
}
