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

    public Grid grid;

    public AudioClip bulletSpawnSound; // Assign in the inspector
    public AudioSource audioSource;


    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void Update()
    {
        //HandleBulletSpawning();
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

    public virtual void SpawnBullet()
    {
        if (audioSource != null && bulletSpawnSound != null)
        {
            audioSource.PlayOneShot(bulletSpawnSound);
        }

        if (bullet != null && isPlaced)
        {
            GameObject spawnedBullet = Instantiate(bullet, transform.position, transform.rotation);

            Bullet bulletComponent = spawnedBullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                // Determine direction and dead zone based on the soldier's tag
                if (gameObject.CompareTag("LeftTeam"))
                {
                    bulletComponent.Initialize(Atk, 100f, Vector3.right); // Bullets move right
                }
                else if (gameObject.CompareTag("RightTeam"))
                {
                    bulletComponent.Initialize(Atk, -100f, Vector3.left); // Bullets move left
                }

                //Debug.Log($"Spawned bullet from {gameObject.tag} with Atk: {bulletComponent.Atk}");
            }
            else
            {
                Debug.LogWarning("Spawned object does not have a Bullet component!");
            }
        }
    }


    public override void OnPlaced()
    {
        base.OnPlaced();
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
