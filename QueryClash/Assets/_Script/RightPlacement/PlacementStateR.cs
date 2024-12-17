using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementStateR : IBuildingStateR
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectDatabaseSOR database;
    GridDataR floorData;
    GridDataR unitData;
    ObjectPlacerR ObjectPlacerR;
    GameObject uiElementToDelete;
    int grade;

    public PlacementStateR(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectDatabaseSOR database,
                          GridDataR floorData,
                          GridDataR unitData,
                          ObjectPlacerR ObjectPlacerR,
                          GameObject uiElementToDelete,
                          int grade)

    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.unitData = unitData;
        this.ObjectPlacerR = ObjectPlacerR;
        this.uiElementToDelete = uiElementToDelete;
        this.grade = grade;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
            throw new System.Exception($"No object with ID {iD}");

    }

    public void Endstate()
    {
        previewSystem.StopShowingPreview();
        //GameObject.Destroy(uiElementToDelete);
    }

    public void OnAction(Vector3Int gridPosition)
    {

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }
        int index = ObjectPlacerR.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition),
            grade);

        GridDataR selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            unitData;
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        Debug.Log("Place at " + gridPosition);
        GameObject.Destroy(uiElementToDelete);
    }

    private bool CheckPlacementValidity(Vector3Int gridPositition, int selectedObjectIndex)
    {
        GridDataR selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : unitData;

        return selectedData.CanPlaceObjectAt(gridPositition, database.objectsData[selectedObjectIndex].Size);

    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }

}
