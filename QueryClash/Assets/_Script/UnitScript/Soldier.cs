using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{

    public float MaxHp;
    public float CurrentHp;
    public float Atk;
    public GameObject bullet;

    public float spawnRate; // How often to spawn bullets (in seconds)
    public float bulletTimer;

    private Grid grid;


    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        
    }

    public void Update()
    {
        HandleBulletSpawning();
    }

    public void HandleBulletSpawning()
    {
        if (isPlaced)
        {
            bulletTimer += Time.deltaTime;
        }

        // Check if it's time to spawn a bullet
        if (bulletTimer >= spawnRate)
        {
            SpawnBullet();
            bulletTimer = 0f; // Reset timer
        }
    }

    public void SpawnBullet()
    {
        if (bullet != null && isPlaced == true)
        {
            GameObject spawnedBullet = Instantiate(bullet, transform.position, transform.rotation);

            // Adjust the spawned bullet's position
            spawnedBullet.transform.position = new Vector3(
                spawnedBullet.transform.position.x + 0.5f,
                spawnedBullet.transform.position.y +0.5f,
                spawnedBullet.transform.position.z + 0.5f
            );
        }
        
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
        SpawnBullet();
        bulletTimer = 1f;
    }

    public virtual void ReduceHp(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            //if (grid == null)
            //{
            //    Debug.LogError("Grid is not assigned to the unit!");
            //    return;
            //}
            // Get the grid position of the unit
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
    }

    

}
