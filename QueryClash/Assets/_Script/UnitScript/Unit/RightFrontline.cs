using UnityEngine;
using FishNet.Object;
using System.Collections;

public class RightFrontline : Soldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;

    public GameObject skillFX;
    public AudioClip skillSound;

    public new void Start()
    {
        base.Start();

        float maxhp = 150f * (1 + score.Value / 1000);
        UpdateSpawnHP(maxhp);

        MaxHp.Value = maxhp;    // Set specific MaxHp for LeftFrontline
        spawnRate = 1.2f;                   // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 10 * (1 + score.Value / 1000);
    }

    [Server]
    void Update()
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
            PlaySoundAndAnimationClient();

            CurrentHp.Value = Mathf.Min(CurrentHp.Value + 50, MaxHp.Value);

            Debug.Log("Skill ended");
            ResetSkill();
        }
    }

    [ObserversRpc]
    public void PlaySoundAndAnimationClient()
    {
        ShowFX();
        audioSource.PlayOneShot(skillSound);
    }

    [Server]
    public void ResetSkill()
    {
        spawnRate = 1.2f; // Reset spawn rate to default
        skillCooldownRemaining = 0f; // Reset cooldown timer
        skillDuration = 0f; // Reset skill duration
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
}