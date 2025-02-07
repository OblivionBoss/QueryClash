using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SinglePlacementSystem : MonoBehaviour
{

    [SerializeField] SingleInputManager inputManager;
    [SerializeField] Grid grid;

    [SerializeField] private ObjectsDatabaseSO database;

    [SerializeField] private GameObject gridVisualization;

    private SingleGridData floorData, unitData;

    [SerializeField] private SinglePreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private SingleObjectPlacer objectPlacer;

    //public GameObject uiElementToDestroy;
    //public int unitID;

    IBuildingState buildingState;
    private void Start()
    {
        //StopPlacement();
        floorData = new();
        unitData = new();
        //gridVisualization.SetActive(true);
    }


    public void StartPlacement(int ID, GameObject uiElementToDelete, float score)
    {
        StopPlacement();
        buildingState = new SinglePlacementState(ID, grid, preview, database, floorData, unitData, objectPlacer, uiElementToDelete, score);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        
    }

    //For Delete Button
    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new SingleRemovingState(grid, preview, floorData, unitData, objectPlacer);
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
        Debug.Log("Show grid position" + gridPosition);
        // Place the structure
        buildingState.OnAction(gridPosition);

        StopPlacement();
    }

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
