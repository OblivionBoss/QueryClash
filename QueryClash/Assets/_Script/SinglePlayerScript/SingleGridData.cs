using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGridData
{
    Dictionary<Vector3Int, SinglePlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placeObjectIndex)
    {
        List<Vector3Int> posistionToOccupy = CalculatePostions(gridPosition, objectSize);
        SinglePlacementData data = new SinglePlacementData(posistionToOccupy, ID, placeObjectIndex);
        foreach (var pos in posistionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePostions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePostions(gridPosition, objectSize);

        foreach (var position in positionToOccupy)
        {
            if (placedObjects.ContainsKey(position))
            {
                return false;
            }
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }

        return placedObjects[gridPosition].PlaceObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

public class SinglePlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlaceObjectIndex { get; private set; }

    public SinglePlacementData(List<Vector3Int> occupiedPositions, int iD, int placeObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlaceObjectIndex = placeObjectIndex;
    }
}