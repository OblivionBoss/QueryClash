using UnityEngine;
using FishNet.Object;

public class LeftFrontline : Soldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;
    private Animator childAnimator;

    public new void Start()
    {
        base.Start();
        MaxHp.Value = 100f * (1 + score / 1000);  // Set specific MaxHp for LeftFrontline
        spawnRate = 1f;                     // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 10 * (1 + score / 1000);
    }

    [Server]
    public void Update()
    {
        if (!ClientManager.Connection.IsHost) return;
        HandleBulletSpawning();
        ActivateSkill();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();

        bulletTimer = 0f;

        if (childAnimator == null) // Reassign if null
        {
            childAnimator = GetComponentInChildren<Animator>();
        }
        if (childAnimator != null)
        {
            childAnimator.SetBool("Shooting", true);
            Debug.Log("Set shooting = true");
        }
        else
        {
            Debug.LogWarning("Animator reference is null in OnPlaced!");
        }
    }

    [Server]
    public void ActivateSkill()
    {
        if (!isPlaced) return; // Do nothing if the soldier is not placed

        // Increment cooldown timer if skill is not active
        if (skillCooldownRemaining < skillCooldown && !timer.isCountingDown.Value)
        {
            skillCooldownRemaining += Time.deltaTime;
        }

        // Activate skill if cooldown has elapsed
        if (skillCooldownRemaining >= skillCooldown && skillDuration == 0)
        {
            Debug.Log("Skill activated");
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

    [Server]
    public void ResetSkill()
    {
        spawnRate = 1f;                 // Reset spawn rate to default
        skillCooldownRemaining = 0f;    // Reset cooldown timer
        skillDuration = 0f;             // Reset skill duration
    }
}