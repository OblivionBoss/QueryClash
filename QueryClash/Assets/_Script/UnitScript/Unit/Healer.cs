using FishNet.Object;
using System.Collections;
using UnityEngine;

public class Healer : Unit
{
    public float healAmount ;    // Amount healed per second
    public float healDuration = 10f;  // Total healing duration
    public float healInterval = 2f;   // Heal every second
    public float healRange = 1.2f;      // Healing radius

    private bool isHealing = false;

    public Grid grid;
    public Timer timer;

    public GameObject specialEffect;
    public AudioClip healingSound;
    public AudioSource healingAudioSource;

    public float elapsedTime = 0f;

    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        timer = GameObject.FindObjectOfType<Timer>();
        healAmount *= 10f + (score.Value / 1000);

        if (specialEffect == null)
        {
            specialEffect = transform.Find("Healing FX")?.gameObject;
        }
        if (healingSound != null && isPlaced)
        {
            PlayHealingSound();
        }
    }

    [Server]
    public void Update()
    {
        if (!ClientManager.Connection.IsHost || !timer.isGameStart.Value) return;

        if (isPlaced && !timer.isCountingDown.Value && !isHealing)
        {
            StartCoroutine(HealOverTime());
        }
    }

    [ObserversRpc]
    public void PlayHealingSound()
    {
        healingAudioSource.clip = healingSound;
        healingAudioSource.loop = true;
        healingAudioSource.Play();
    }

    [ObserversRpc]
    private void StopHealingSound()
    {
        if (healingAudioSource.isPlaying)
        {
            healingAudioSource.Stop();
        }
    }

    [Server]
    private IEnumerator HealOverTime()
    {
        isHealing = true;
        specialEffect.SetActive(true);
        elapsedTime = 0f; // Reset elapsed time when healing starts

        while (elapsedTime < healDuration) // Loop while heal duration is not reached
        {
            HealNearbyUnits();
            elapsedTime += healInterval; // Increase time only if healing happens
            yield return new WaitForSeconds(healInterval); // Wait for the next heal tick
        }

        isHealing = false;

        Debug.Log($"{gameObject.name} finished healing and has been destroyed.");
        Vector3Int gridPosition = grid.WorldToCell(transform.position);
        RemoveUnit(gridPosition);
        StopHealingSound();
        //Destroy(gameObject); // Destroy the healer after healing duration ends
    }

    [Server]
    private void HealNearbyUnits()
    {
        Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);  // Define the offset
        Vector3 adjustedPosition = transform.position + offset;  // Apply the offset to the position

        // Use the adjusted position for the healing range
        Collider[] hitColliders = Physics.OverlapSphere(adjustedPosition, healRange);

        foreach (Collider collider in hitColliders)
        {
            Soldier soldier = collider.GetComponent<Soldier>();
            if (soldier != null && soldier.CurrentHp.Value > 0 && soldier.CurrentHp.Value < soldier.MaxHp.Value && soldier.CompareTag(gameObject.tag))
            {
                Heal(soldier);
            }
        }
    }

    [Server]
    private void Heal(Soldier soldier)
    {
        soldier.HealingHp(healAmount);
        Debug.Log($"{soldier.gameObject.name} healed to {soldier.CurrentHp} HP.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set color to green for the healing range
        //Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f); // Define the offset
        Gizmos.DrawWireSphere(transform.position, healRange); // Draw healing range sphere with offset
    }
}