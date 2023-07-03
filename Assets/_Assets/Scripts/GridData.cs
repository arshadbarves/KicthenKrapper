using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition)
    {
        Vector3Int positionToOccupy = CalculatePositions(gridPosition);
        PlacementData data = new(positionToOccupy);
        if (placedObjects.ContainsKey(positionToOccupy))
            throw new Exception($"Dictionary already contains this cell position {positionToOccupy}");
        placedObjects[positionToOccupy] = data;
    }

    private Vector3Int CalculatePositions(Vector3Int gridPosition)
    {
        Vector3Int returnVal = gridPosition + new Vector3Int(1, 0, 1);
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition)
    {
        Vector3Int pos = CalculatePositions(gridPosition);
        if (placedObjects.ContainsKey(pos))
            return false;
        return true;
    }
}

public class PlacementData
{
    public Vector3Int occupiedPositions;

    public PlacementData(Vector3Int occupiedPositions)
    {
        this.occupiedPositions = occupiedPositions;
    }
}