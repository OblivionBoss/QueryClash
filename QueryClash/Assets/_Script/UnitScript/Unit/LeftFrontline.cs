using UnityEngine;
using FishNet.Object;

public class LeftFrontline : Soldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;

    public GameObject skillFX;
    public AudioClip skillSound;

    public new void Start()
    {
        base.Start();

        float maxhp = 100f * (1 + score.Value / 1000);
        UpdateSpawnHP(maxhp);

        MaxHp.Value = maxhp;  // Set specific MaxHp for LeftFrontline
        spawnRate = 1f;                     // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 10 * (1 + score.Value / 1000);
    }

    [Server]
    public void Update()
    {
        if (!ClientManager.Connection.IsHost || !timer.isGameStart.Value) return;
        HandleBulletSpawning();
        ActivateSkill();
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
            spawnRate = 0.3f;
            skillDuration += Time.deltaTime; // Start counting skill duration

            PlaySoundAndAnimationClient();
        }
        else if (skillDuration > 0) // Skill is active
        {
            skillDuration += Time.deltaTime;

            // Check if skill duration has ended
            if (skillDuration >= 5f)
            {
                ResetSkill();
                Debug.Log("Skill ended");
            }
        }
    }

    [ObserversRpc]
    public void PlaySoundAndAnimationClient()
    {
        skillFX.SetActive(true);
        audioSource.PlayOneShot(skillSound);
    }

    [Server]
    public void ResetSkill()
    {
        spawnRate = 1f;                 // Reset spawn rate to default
        skillCooldownRemaining = 0f;    // Reset cooldown timer
        skillDuration = 0f;             // Reset skill duration
        StopAnimationClient();
    }

    [ObserversRpc]
    public void StopAnimationClient()
    {
        skillFX.SetActive(false);
    }
}