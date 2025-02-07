using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SingleUnit : MonoBehaviour 
{
    private SinglePlacementSystem placementSystem;
    public bool isPlaced = false;
    public float score;
    public bool isBase=false;

    public event Action OnDeath;
    public void Start()
    {
        placementSystem = GameObject.FindObjectOfType<SinglePlacementSystem>();

    }

    // Update is called once per frame
    public void Update()
    {

    }


    public void RemoveUnit(Vector3Int gridPosition)
    {
        // Call RemovingState logic directly
        placementSystem?.RemoveUnitAt(gridPosition);

        OnDeath?.Invoke();

        // Destroy the GameObject
        Destroy(gameObject);
        Debug.Log($"Unit at {gridPosition} has been removed.");
        
    }

    public virtual void OnPlaced()
    {
        isPlaced = true;

    }

    public void SetScore(float score)
    {
        this.score = score;
    }


}
