using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBurst : SingleSoldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;

    public GameObject skillFX;
    public AudioClip skillSound;
    void Start()
    {
        base.Start();
        MaxHp = 100f * Mathf.Pow(1 + score / 500f, 2);         // Set specific MaxHp for LeftFrontline
        spawnRate = 1f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 10 * Mathf.Pow(1 + score / 500f, 2);
        HealthBarUpdate();
    }

    void Update()
    {
        if (baseManager.gameEnd) return;
        
        HandleBulletSpawning();
        ActivateSkill();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
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
        if (skillCooldownRemaining >= skillCooldown && skillDuration == 0)
        {
            Debug.Log("Skill activated");
            skillFX.SetActive(true);
            audioSource.PlayOneShot(skillSound);
            spawnRate = 0.5f;
            skillDuration += Time.deltaTime; // Start counting skill duration
        }
        else if (skillDuration > 0) // Skill is active
        {
            skillDuration += Time.deltaTime;

            // Check if skill duration has ended
            if (skillDuration >= 3f)
            {
                ResetSkill();
                Debug.Log("Skill ended");
            }
        }
    }

    public void ResetSkill()
    {
        spawnRate = 1f; // Reset spawn rate to default
        skillCooldownRemaining = 0f; // Reset cooldown timer
        skillDuration = 0f; // Reset skill duration
        skillFX.SetActive(false);

    }


}
