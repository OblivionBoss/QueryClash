using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlacementSystemR : MonoBehaviour
{

    [SerializeField] InputManager inputManager;
    [SerializeField] Grid grid;

    [SerializeField] private ObjectDatabaseSOR database;

    [SerializeField] private GameObject gridVisualization;

    private GridDataR floorData, unitData;

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacerR objectPlacer;

    //public GameObject uiElementToDestroy;
    //public int unitID;

    IBuildingStateR buildingState;
    private void Start()
    {
        //StopPlacement();
        floorData = new();
        unitData = new();
        //gridVisualization.SetActive(true);
    }


    public void StartPlacement(int ID, GameObject uiElementToDelete, int Grade)
    {
        StopPlacement();
        buildingState = new PlacementStateR(ID, grid, preview, database, floorData, unitData, objectPlacer, uiElementToDelete, Grade);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;

    }

    //For Delete Button
    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingStateR(grid, preview, floorData, unitData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
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
        if (buildingState == null)
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
        if (buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }


    }

    //For delete unit soldier when it died
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
