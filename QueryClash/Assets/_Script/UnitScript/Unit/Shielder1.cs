using UnityEngine;
using FishNet.Object;

public class Shielder1 : Soldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;
    

    public GameObject skillFX;
    public AudioClip skillSound;

    private float Defence = 0f;

    public new void Start()
    {
        base.Start();
        MaxHp.Value = 1000f * (1 + score.Value / 1000); // Set specific MaxHp for LeftFrontline
        spawnRate = 0f;                     // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 0f;
    }

    [Server]
    void Update()
    {
        if (ClientManager.Connection.IsHost)
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
            PlaySoundAndAnimationClient();
            Defence = 0.5f;
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
    public override void ReduceHp(float damage)
    {
        CurrentHp.Value = Mathf.Max(0, CurrentHp.Value - damage * (1 - Mathf.Min(0.9f, Defence)));
        if (CurrentHp.Value <= 0)
        {
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
        //ClientHealthBarUpdate();
    }

    [Server]
    public void ResetSkill()
    {
        Defence = 0f;                   // Reset Defence to default
        skillCooldownRemaining = 0f;    // Reset cooldown timer
        skillDuration = 0f;             // Reset skill duration
        StopAnimationClient();
    }
    
    [ObserversRpc]
    public void PlaySoundAndAnimationClient()
    {
        skillFX.SetActive(true);
        audioSource.PlayOneShot(skillSound);
    }

    [ObserversRpc]
    public void StopAnimationClient()
    {
        skillFX.SetActive(false);
    }
}