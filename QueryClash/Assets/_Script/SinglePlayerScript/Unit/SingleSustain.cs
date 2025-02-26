using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSustain : SingleSoldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;

    public GameObject skillFX;
    public AudioClip skillSound;
    void Start()
    {
        base.Start();
        MaxHp = 150f * (1 + score/1000);         // Set specific MaxHp for LeftFrontline
        spawnRate = 1.2f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 10 * (1 + score / 1000);
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
            ShowFX();
            audioSource.PlayOneShot(skillSound);
            CurrentHp += 50;
            if (CurrentHp > MaxHp)
            {
                CurrentHp = MaxHp;
            }

            Debug.Log("Skill ended");
            ResetSkill();
            
        }
     
    }

    public void ShowFX()
    {
        skillFX.SetActive(true);
        StartCoroutine(HideFXAfterDelay(3f)); // This runs in the background
        Debug.Log("This will run immediately after setting FX active");
    }

    private IEnumerator HideFXAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Waits for 3 seconds
        skillFX.SetActive(false);
        Debug.Log("FX is now hidden");
    }
    public void ResetSkill()
    {
        spawnRate = 1.2f; // Reset spawn rate to default
        skillCooldownRemaining = 0f; // Reset cooldown timer
        skillDuration = 0f; // Reset skill duration
        

    }
}
