using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEnemySpawner : MonoBehaviour
{
    [SerializeField]
    private SingleObjectPlacer objectPlacer; // Reference to the ObjectPlacer
    [SerializeField]
    private List<Vector3> availablePositions = new(); // List of spawnable positions
    [SerializeField]
    private List<int> enemyIds = new() { 201, 202, 203, 204, 205, 206, 207, 208, 209, 210 }; // Enemy IDs

    [SerializeField]
    private ObjectsDatabaseSO database; // Reference to the object database
    [SerializeField]
    private float spawnInterval = 10f; // Time between spawns

    private string level;
     
    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        InitializeAvailablePositions();
        StartCoroutine(SpawnEnemies());
    }

    private void InitializeAvailablePositions()
    {
        // Populate available positions in the range (x: 0-5, z: -2 to 2, y: 0)
        for (int x = 0; x <= 5; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                availablePositions.Add(new Vector3Int(x, 0, z));
            }
        }
    }
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(spawnInterval); // Wait for the next spawn
            SpawnEnemy(); // Spawn one enemy
        }
    }

    private int GetRandomEnemyScore()
    {
        int roll = UnityEngine.Random.Range(0, 100); // Random number from 0 to 99

        if (roll < 50) return 0;       // 50% chance
        if (roll < 80) return 100;     // 30% chance
        if (roll < 90) return 500;     // 10% chance
        if (roll < 97) return 900;     // 9% chance
        return 1250;                   // 1% chance
    }

    private void SpawnEnemy()
    {
        if (availablePositions.Count == 0)
        {
            Debug.LogWarning("No available positions to spawn an enemy.");
            return;
        }

        // Randomly select an available position
        Vector3 spawnPosition = availablePositions[Random.Range(0, availablePositions.Count)];

        // Randomly select an enemy ID
        //int randomEnemyIndex = Random.Range(0, enemyIds.Count);
        int enemyId = enemyIds[Random.Range(0, enemyIds.Count)];
        int enemyScore = GetRandomEnemyScore();//Random from score = 0 for 40%, score = 100 for 40%, score = 500 for 10 %, score = 900 for 7%, score = 1250 for 3%
        Debug.Log($"Spawned enemy ID: {enemyId} at position: {spawnPosition}");

        // Get the enemy prefab from the database
        ObjectData enemyData = database.objectsData.Find(data => data.ID == enemyId);
        if (enemyData == null)
        {
            Debug.LogError($"No enemy found with ID {enemyId}.");
            return;
        }

        // Place the enemy at the selected position
        int placedIndex = objectPlacer.PlaceObject(enemyData.Prefab, spawnPosition, enemyScore);

        // Remove the position from the available list
        availablePositions.Remove(spawnPosition);

        // Handle the placed unit
        GameObject placedObject = objectPlacer.GetPlacedObject(placedIndex);
        if (placedObject != null)
        {
            SingleUnit unit = placedObject.GetComponent<SingleUnit>();
            if (unit != null)
            {
                Vector3Int gridPosition = Vector3Int.RoundToInt(spawnPosition);

                // Listen for the unit's death to reclaim the position
                unit.OnDeath += () => OnUnitDeath(gridPosition);
            }
        }
    }

    private void OnUnitDeath(Vector3Int gridPosition)
    {
        Vector3 worldPosition = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);

        // Reclaim the position for future spawns
        if (!availablePositions.Contains(worldPosition))
        {
            availablePositions.Add(worldPosition);
        }
    }

    public void SetLevel(string level)
    {
        this.level = level;
    }
}
