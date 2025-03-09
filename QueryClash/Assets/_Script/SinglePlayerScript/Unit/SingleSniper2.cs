using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSniper2 : SingleSoldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;

    private bool skillUsing;
    public GameObject specialBullet;
    void Start()    
    {
        base.Start();
        MaxHp = 100f * Mathf.Pow(1 + score / 500f, 2);         // Set specific MaxHp for LeftFrontline
        spawnRate = 2f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 25 * Mathf.Pow(1 + score / 500f, 2);
        HealthBarUpdate();
    }

    void Update()
    {
        //base.Update();
        if (baseManager.gameEnd) return;
        HandleBulletSpawning();
        ActivateSkill();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
        skillUsing = false;
    }

    public override void SpawnBullet()
    {
        if (audioSource != null && bulletSpawnSound != null)
        {
            audioSource.PlayOneShot(bulletSpawnSound);
        }

        if (bullet != null && isPlaced && skillUsing == false)
        {
            GameObject spawnedBullet = Instantiate(bullet, transform.position, transform.rotation);

            SingleBullet bulletComponent = spawnedBullet.GetComponent<SingleBullet>();
            if (bulletComponent != null)
            {
                // Determine direction and dead zone based on the soldier's tag
                if (gameObject.CompareTag("LeftTeam"))
                {
                    bulletComponent.Initialize(Atk, 100f, Vector3.right, "RightTeam", "LeftTeam"); // Bullets move right
                }
                else if (gameObject.CompareTag("RightTeam"))
                {
                    bulletComponent.Initialize(Atk, -100f, Vector3.left, "LeftTeam", "RightTeam"); // Bullets move left
                }

                //Debug.Log($"Spawned bullet from {gameObject.tag} with Atk: {bulletComponent.Atk}");
            }
            else
            {
                Debug.LogWarning("Spawned object does not have a Bullet component!");
            }
        }
        else if (bullet != null && isPlaced && skillUsing == true)
        {
            GameObject spawnedBullet = Instantiate(specialBullet, transform.position, transform.rotation);

            SingleBullet bulletComponent = spawnedBullet.GetComponent<SingleBullet>();
            if (bulletComponent != null)
            {
                // Determine direction and dead zone based on the soldier's tag
                if (gameObject.CompareTag("LeftTeam"))
                {
                    bulletComponent.Initialize(Atk, 100f, Vector3.right, "RightTeam", "LeftTeam"); // Bullets move right
                }
                else if (gameObject.CompareTag("RightTeam"))
                {
                    bulletComponent.Initialize(Atk, -100f, Vector3.left, "LeftTeam", "RightTeam"); // Bullets move left
                }

                //Debug.Log($"Spawned bullet from {gameObject.tag} with Atk: {bulletComponent.Atk}");
            }
            else
            {
                Debug.LogWarning("Spawned object does not have a Bullet component!");
            }
            ResetSkill();
        }
    }



    public void ActivateSkill()
    {
        if (!isPlaced)
            return; // Do nothing if the soldier is not placed

        // Increment cooldown timer if skill is not active
        if (skillCooldownRemaining < skillCooldown && !timer.isCountingDown)
        {
            skillCooldownRemaining += Time.deltaTime;
        }

        // Activate skill if cooldown has elapsed
        if (skillCooldownRemaining >= skillCooldown)
        {
            Debug.Log("Skill activated");
            skillUsing = true;

        }
    }

    public void ResetSkill()
    {
        skillCooldownRemaining = 0f; // Reset cooldown timer
        skillUsing = false;
    }
}
