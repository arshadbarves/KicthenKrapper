using System.Collections.Generic;
using UnityEngine;

public class CounterGenerator : MonoBehaviour, IGenerator
{
    public Vector2Int playgroundSize;
    public Vector3 prefabSize;
    public float prefabSizeMultiplier = 1;

    [Header("Counters")]
    public List<PSGAsset> counterPrefabs;

    private List<int> usedCounters;
    private List<Cell> eligibleCells;

    public void ClearMeshes()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
    }

    public void Generate(List<Cell> cells, Vector2Int playgroundSize)
    {
        this.playgroundSize = playgroundSize;
        ClearMeshes();
        usedCounters = new List<int>();
        eligibleCells = new List<Cell>();

        foreach (var cell in cells)
        {
            if (cell.cellTag == CellTag.Counter)
            {
                eligibleCells.Add(cell);
            }
        }

        while (eligibleCells.Count > 0)
        {
            Cell cell = eligibleCells[Random.Range(0, eligibleCells.Count)];
            if (cell.prefab != null)
            {
                eligibleCells.Remove(cell);
                continue;
            }

            eligibleCells.Remove(cell);

            List<PSGAsset> LimitedPrefabs = counterPrefabs.FindAll(p => p.isLimited == true && usedCounters.FindAll(id => id == p.id).Count < p.limit);
            List<PSGAsset> NonLimitedPrefabs = counterPrefabs.FindAll(p => p.isLimited == false);

            PSGAsset prefab = null;
            if (cell.isCorner)
            {
                prefab = NonLimitedPrefabs[Random.Range(0, NonLimitedPrefabs.Count)];
            }
            else
            {
                if (LimitedPrefabs.Count > 0)
                {
                    prefab = LimitedPrefabs[Random.Range(0, LimitedPrefabs.Count)];
                }
                else if (NonLimitedPrefabs.Count > 0)
                {
                    prefab = NonLimitedPrefabs[Random.Range(0, NonLimitedPrefabs.Count)];
                }
                else
                {
                    print("No prefabs found");
                    return;
                }
            }

            if (prefab == null)
            {
                print("No suitable prefab found");
                continue;
            }

            var position = cell.position;

            // Rotate the prefab to face the direction of the cell (north, east, south, west), but only 90 degree increments.
            Quaternion rotation;

            rotation = cell.rotation;


            var instance = Instantiate(prefab.prefab, position, rotation, transform);
            instance.transform.localScale = prefabSize * prefabSizeMultiplier;
            instance.name = prefab.prefab.name;
            cell.prefab = prefab;
            usedCounters.Add(prefab.id);
        }

        // Check if all limited prefabs have been used
        foreach (var prefab in counterPrefabs)
        {
            if (prefab.isLimited && usedCounters.FindAll(id => id == prefab.id).Count < prefab.limit)
            {
                print("Not enough prefabs of type " + prefab.prefab.name + " found");
            }
        }
    }
}
