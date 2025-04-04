using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    SinglePreviewSystem previewSystem;
    SingleGridData floorData;
    SingleGridData unitData;
    SingleObjectPlacer objectPlacer;

    public SingleRemovingState(Grid grid,
                         SinglePreviewSystem previewSystem,
                         SingleGridData floorData,
                         SingleGridData unitData,
                         SingleObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.unitData = unitData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void Endstate() // Stop preview silouhette, when object was delete
    {
        previewSystem.StopShowingPreview();
        
    }

    public void OnAction(Vector3Int gridPosition) // Using while previewing where to delete object
    {
        SingleGridData selectedData = null;
        if (unitData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = unitData;
        }
        else if (unitData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = null;
        }

        if (selectedData == null)
        {

        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveOBjectAt(gameObjectIndex);
        }

        Vector3 cellPostion = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPostion, CheckIfSelectionIsValid(gridPosition));

    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition) //Check if we can actually delete object or not
    {
        return (unitData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition) // Update to where has preview image, according to mouse cursor position
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
