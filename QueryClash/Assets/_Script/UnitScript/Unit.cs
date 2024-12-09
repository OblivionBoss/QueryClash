using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private PlacementSystem placementSystem;
    public bool isPlaced = false;
    public float grade;

    public void Start()
    {
        placementSystem = GameObject.FindObjectOfType<PlacementSystem>();

    }

    // Update is called once per frame
    public void Update()
    {

    }


    public void RemoveUnit(Vector3Int gridPosition)
    {
        // Call RemovingState logic directly
        if (placementSystem != null)
        {
            placementSystem.RemoveUnitAt(gridPosition);
        }

        // Destroy the unit GameObject
        Destroy(gameObject);
        Debug.Log("Call Remove Unit Function");
    }

    public virtual void OnPlaced()
    {
        isPlaced = true;

    }

    public void SetGrade(int Grade)
    {
        this.grade = Grade;
    }


}
