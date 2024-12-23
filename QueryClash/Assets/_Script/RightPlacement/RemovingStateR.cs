using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingStateR : IBuildingStateR
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridDataR floorData;
    GridDataR unitData;
    ObjectPlacerR objectPlacer;

    public RemovingStateR(Grid grid,
                         PreviewSystem previewSystem,
                         GridDataR floorData,
                         GridDataR unitData,
                         ObjectPlacerR objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.unitData = unitData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void Endstate()
    {
        previewSystem.StopShowingPreview();

    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridDataR selectedData = null;
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

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return (unitData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
