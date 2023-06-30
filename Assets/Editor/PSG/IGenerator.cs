using System.Collections.Generic;
using UnityEngine;

public interface IGenerator
{
    void Generate(List<Cell> cells, Vector2Int gridSize);
    void ClearMeshes();
}
