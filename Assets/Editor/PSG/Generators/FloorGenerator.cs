using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour, IGenerator
{
    public PSGAsset[] prefabs;

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
        var prefabSize = prefabs[0].bounds.size;
        var seed = Random.Range(0, 1000000);
        Random.InitState(seed);

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            var prefab = ChoosePrefab(PrefabType.Floor, cell.prefabTag);
            var position = cell.position;
            var rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);

            var instance = Instantiate(prefab.prefab, position, rotation, transform);
            instance.name = prefab.prefab.name;
        }
    }



    // Generate a random prefab based on the prefab type and prefab tag
    public PSGAsset ChoosePrefab(PrefabType prefabType, PrefabTag prefabTag)
    {
        float totalProbability = 0;

        foreach (var prefab in prefabs)
        {
            if (prefab.prefabType == prefabType && prefab.prefabTag == prefabTag)
            {
                totalProbability += prefab.probability;
            }
        }

        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0;

        foreach (var prefab in prefabs)
        {
            if (prefab.prefabType == prefabType && prefab.prefabTag == prefabTag)
            {
                cumulativeProbability += prefab.probability;

                if (randomValue <= cumulativeProbability)
                {
                    return prefab;
                }
            }
        }

        // If no suitable prefab found, return null
        return prefabs[0];
    }

}
