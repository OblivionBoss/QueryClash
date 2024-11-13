using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleUnit : MonoBehaviour
{
    /* Unit Part */
    public float MaxHp;
    public float CurrentHp;

    private PlacementSystem placementSystem;
    private Grid grid;
    private bool isPlaced = false;

    /* Bullet Part */
    public GameObject bullet;     // The bullet prefab to be spawned
    public float spawnRate = 1f; // How often to spawn bullets (in seconds)
    private float bulletTimer = -1;

    void Start()
    {
        // Initialize HP
        //CurrentHp = MaxHp;

        // Reference to the PlacementSystem in the scene
        placementSystem = GameObject.FindObjectOfType<PlacementSystem>();
        grid = GameObject.FindObjectOfType<Grid>();
    }

    void Update()
    {
        HandleBulletSpawning();
    }

    // Method to handle bullet spawning logic
    private void HandleBulletSpawning()
    {
        bulletTimer += Time.deltaTime;

        // Check if it's time to spawn a bullet
        if (bulletTimer >= spawnRate)
        {
            SpawnBullet();
            bulletTimer = 0f; // Reset timer
        }
    }

    // Instantiate the bullet at the current position of the unit
    private void SpawnBullet()
    {
        if (bullet != null && isPlaced == true)
        {
            Instantiate(bullet, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("Bullet prefab is not assigned.");
        }
    }

    // Method to reduce HP of the unit
    public void ReduceHp(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            // Get the grid position of the unit
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
    }

    // Method to remove the unit from the grid and destroy it
    private void RemoveUnit(Vector3Int gridPosition)
    {
        // Call RemovingState logic directly
        if (placementSystem != null)
        {
            placementSystem.RemoveUnitAt(gridPosition);
        }

        // Destroy the unit GameObject
        Destroy(gameObject);
    }

    public void OnPlaced()
    {
        isPlaced = true;

        // Initialize HP
        CurrentHp = MaxHp;

        // Start bullet spawning only after the unit is placed
        bulletTimer = 0f;

        gameObject.layer = 3; // Sets the GameObject to layer 6

    }

}
