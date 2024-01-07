using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorativeGenerator : MonoBehaviour, IGenerator
{
    public List<GameObject> decorativePrefabs;

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
        float minimumDistance = 5f;

        ClearMeshes();

        List<Vector3> occupiedPositions = new List<Vector3>();

        foreach (var cell in cells)
        {
            if (cell.cellTag == CellTag.None && cell.prefab == null && cell.prefabTag == PrefabTag.Outside)
            {
                Vector3 position = cell.position;
                bool canInstantiate = true;

                foreach (Vector3 occupiedPosition in occupiedPositions)
                {
                    if (Vector3.Distance(position, occupiedPosition) < minimumDistance)
                    {
                        canInstantiate = false;
                        break;
                    }
                }

                if (canInstantiate)
                {
                    GameObject decorative = Instantiate(decorativePrefabs[Random.Range(0, decorativePrefabs.Count)], transform);
                    decorative.transform.position = position;
                    decorative.transform.rotation = cell.rotation;

                    occupiedPositions.Add(position);
                }
            }
        }
    }
}
