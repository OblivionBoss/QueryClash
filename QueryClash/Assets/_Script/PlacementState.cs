using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData floorData;
    GridData unitData;
    ObjectPlacer objectPlacer;
    GameObject uiElementToDelete;
    int grade;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData floorData,
                          GridData unitData,
                          ObjectPlacer objectPlacer,
                          GameObject uiElementToDelete,
                          int grade)
                          
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.unitData = unitData;
        this.objectPlacer = objectPlacer;
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
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition),
            grade);

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
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
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : unitData;

        return selectedData.CanPlaceObjectAt(gridPositition, database.objectsData[selectedObjectIndex].Size);

    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }

}
