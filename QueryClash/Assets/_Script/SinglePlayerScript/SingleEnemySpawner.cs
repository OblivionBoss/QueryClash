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
    private float spawnInterval; // Time between spawns
    private float[] newSpawnTime;
    [SerializeField]
    private int level;

    public SingleTimer timer;

    public BaseManager baseManager;

    private int[] rollChance;
    
    
    private void Start()
    {
        SetLevelRollChanceAndSpawntime(SingleSceneManager.singleSceneManager.difficulty); 
        baseManager = GameObject.FindObjectOfType<BaseManager>();
        SetLevelRollChanceAndSpawntime(level);
        Random.InitState(System.DateTime.Now.Millisecond);
        InitializeAvailablePositions();
        StartCoroutine(SpawnEnemies());
        StartCoroutine(CheckSpawnTime());   
    }

    

    private void SetSpawnlTime(float time)
    {
        spawnInterval = time;
        Debug.Log($"New spawntime = {spawnInterval}");
    }
    private IEnumerator CheckSpawnTime()
    {
        float[] thresholds = { 300f, 600f, 900f, 1200f };
        int lastAppliedIndex = -1; // Track last applied spawn time

        while (true)
        {
            float elapsed = timer.elapsedTime;

            for (int i = thresholds.Length - 1; i >= 0; i--) // Check from highest to lowest threshold
            {
                if (elapsed >= thresholds[i] && lastAppliedIndex < i)
                {
                    Debug.Log($"Applying spawn time change at {elapsed}s for threshold {thresholds[i]}");
                    SetSpawnlTime(newSpawnTime[i]);
                    lastAppliedIndex = i; // Update last applied
                    break; // Apply only the most recent threshold
                }
            }

            if (elapsed >= 1200f) yield break; // Stop checking after last threshold

            yield return new WaitForSeconds(1f); // Check every second
        }
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
        while (baseManager.gameEnd == false)
        {
            yield return new WaitForSeconds(spawnInterval); // Wait for the next spawn
            SpawnEnemy(); // Spawn one enemy
        }
    }

    private int GetRandomEnemyScore()
    {
        int roll = UnityEngine.Random.Range(0, 100); // Random number from 0 to 99

        if (roll < rollChance[0]) return 0;       // 50% chance
        else if (roll < rollChance[1]) return 100;     // 30% chance
        else if (roll < rollChance[2]) return 500;     // 10% chance
        else if (roll < rollChance[3]) return 900;     // 9% chance
        else return 1250;                   // 1% chance
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

    //public void SetLevel(int level)
    //{
    //    this.level = level;
    //}

    public void SetLevelRollChanceAndSpawntime(int level)
    {
        this.level = level;
        if (level == 0)
        {
            rollChance = new int[] { 50, 80, 90, 97 };
            spawnInterval = 120f;
            
        }
        else if (level == 1)
        {
            rollChance = new int[] { 40, 80, 90, 97 };
            spawnInterval = 100f;
        }
        else if (level == 2)
        {
            rollChance = new int[] { 10, 50, 80, 95 };
            spawnInterval = 80f;
        }
        newSpawnTime = new float[] {spawnInterval * 0.9f, spawnInterval * 0.8f, spawnInterval * 0.75f, spawnInterval * 0.6f, spawnInterval * 0.5f};
        Debug.Log($"Enemy level is {level}, spawnInterval is {spawnInterval}");
    }
}
