using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SingleSoldier : SingleUnit
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

    public SingleTimer timer;

    public Image healthBar;

    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        timer = GameObject.FindObjectOfType<SingleTimer>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        FindHealthBar();

    }

    public void Update()
    {
        //HandleBulletSpawning();
    }

    public void HandleBulletSpawning()
    {
        if (isPlaced && !timer.isCountingDown)
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
        if (audioSource != null && bulletSpawnSound != null && timer.isCountingDown == false)
        {
            audioSource.PlayOneShot(bulletSpawnSound);
        }

        if (bullet != null && isPlaced && timer.isCountingDown == false)
        {
            GameObject spawnedBullet = Instantiate(bullet, transform.position, transform.rotation);

            SingleBullet bulletComponent = spawnedBullet.GetComponent<SingleBullet>();
            if (bulletComponent != null)
            {
                // Determine direction and dead zone based on the soldier's tag
                if (gameObject.CompareTag("LeftTeam"))
                {
                    bulletComponent.Initialize(Atk, 15f, Vector3.right, "RightTeam", "LeftTeam"); // Bullets move right
                }
                else if (gameObject.CompareTag("RightTeam"))
                {
                    bulletComponent.Initialize(Atk, -15f, Vector3.left, "LeftTeam", "RightTeam"); // Bullets move left
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
        healthBar.fillAmount = CurrentHp / MaxHp;
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

    public virtual void HealingHp(float heal)
    {
        this.CurrentHp += heal;
        if(CurrentHp >= MaxHp)
        {
            CurrentHp = MaxHp;
        }
        healthBar.fillAmount = CurrentHp / MaxHp;
    }

    public virtual void FindHealthBar()
    {
        if (healthBar == null)
        {
            string healthBarPath = gameObject.CompareTag("LeftTeam") ?
                "Left Healthbar Canvas/HealthBar" :
                "Right Healthbar Canvas/HealthBar";

            healthBar = transform.Find(healthBarPath)?.GetComponent<Image>();

            if (healthBar == null)
            {
                Debug.LogError($"HealthBar UI not found for {gameObject.name} (Tag: {gameObject.tag})");
            }
        }
    }




}
