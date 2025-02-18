using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SingleUnit : MonoBehaviour 
{
    private SinglePlacementSystem placementSystem;
    public bool isPlaced = false;
    public float score;
    public float grade;
    public bool isBase=false;
    private Renderer showGradeRenderer;
    public event Action OnDeath;
    public void Start()
    {
        placementSystem = GameObject.FindObjectOfType<SinglePlacementSystem>();

        // Find the child object named "ShowGrade" and get its Renderer
        Transform showGradeTransform = transform.Find("ShowGrade");
        if (showGradeTransform != null)
        {
            showGradeRenderer = showGradeTransform.GetComponent<Renderer>();
        }

        SetGrade();
        UpdateGradeColor();
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

    private void SetGrade()
    {
        if (score > 1200f) grade = 5f;
        else if (score > 800f) grade = 4f;
        else if (score > 400f) grade = 3f;
        else if (score > 0f) grade = 3f;
        else grade = 1f;
    }

    private void UpdateGradeColor()
    {
        if (showGradeRenderer == null) return;

        Color newColor = Color.white; // Default color

        switch (grade)
        {
            case 5f: newColor = new Color(1f, 0.743384f, 0f); break;
            case 4f: newColor = new Color(0.7693181f, 0f, 1f); break;
            case 3f: newColor = new Color(0f, 0.5488603f, 1f); break;
            case 2f: newColor = new Color(0f, 0.7987421f, 0.07062242f); break;
            case 1f: newColor = new Color(0.6289307f, 0.6289307f, 0.6289307f); break;
        }

        showGradeRenderer.material.color = newColor;
    }

}
