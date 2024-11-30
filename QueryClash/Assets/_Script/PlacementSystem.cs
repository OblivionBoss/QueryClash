using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField] InputManager inputManager;
    [SerializeField] Grid grid;

    [SerializeField] private ObjectsDatabaseSO database;

    [SerializeField] private GameObject gridVisualization;

    private GridData floorData, unitData;

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;




    IBuildingState buildingState;
    private void Start()
    {
        //StopPlacement();
        floorData = new();
        unitData = new();
        gridVisualization.SetActive(true);
    }


    public void StartPlacement( int ID )
    {
        //StopPlacement();
        
        buildingState = new PlacementState(ID, grid, preview, database, floorData, unitData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, unitData, objectPlacer);

    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        // Place the structure
        buildingState.OnAction(gridPosition);

        StopPlacement();
    }


    //private bool CheckPlacementValidity(Vector3Int gridPositition, int selectedObjectIndex)
    //{
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : unitData;

    //    return selectedData.CanPlaceObjectAt(gridPositition, database.objectsData[selectedObjectIndex].Size);

    //}

    private void StopPlacement()
    {
        if(buildingState == null)
        {
            return;
        }
        //gridVisualization.SetActive(false);
        buildingState.Endstate();  
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if(buildingState == null)
        {
            return ; 
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

        
    }

    public void RemoveUnitAt(Vector3Int gridPosition)
    {
        // Check if there is a unit at the specified grid position
        if (unitData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            // Get the index of the unit to be removed
            int gameObjectIndex = unitData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex != -1)
            {
                // Remove unit data and destroy the GameObject
                unitData.RemoveObjectAt(gridPosition);
                objectPlacer.RemoveOBjectAt(gameObjectIndex);
            }
        }
    }

}
