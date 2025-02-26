using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHealer : SingleUnit
{
    private float healAmount;    // Amount healed per second
    public float healDuration = 10f;  // Total healing duration
    public float healInterval = 1f;   // Heal every second
    public float healRange = 1.2f;      // Healing radius

    private bool isHealing = false;

    public AudioClip HealingSound; // Assign in the inspector
    public AudioSource healingAudioSource;

    public Grid grid;
    public SingleTimer timer;

    public GameObject specialEffect;

    public float elapsedTime = 0f;

    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        timer = GameObject.FindObjectOfType<SingleTimer>();
        healAmount *= 10 + (score / 1000);

        if (specialEffect == null)
        {
            specialEffect = transform.Find("Healing FX")?.gameObject;
        }
        if (HealingSound != null && isPlaced)
        {
            PlayHealingSound();
        }
    }

    public void Update()
    {
        if (isPlaced && !timer.isCountingDown && !isHealing)
        {
            StartCoroutine(HealOverTime());
        }

    }
    private void PlayHealingSound()
    {
        healingAudioSource.clip = HealingSound;
        healingAudioSource.loop = true;
        healingAudioSource.Play();
    }

    private void StopHealingSound()
    {
        if (healingAudioSource.isPlaying)
        {
            healingAudioSource.Stop();
        }
    }
    public override void OnPlaced()
    {
        base.OnPlaced();

        Debug.Log($"this unit {healAmount}");
        //if (!isHealing) 
        //{
        //    StartCoroutine(HealOverTime());
        //
    }


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


    private void HealNearbyUnits()
    {
        Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);  // Define the offset
        Vector3 adjustedPosition = transform.position + offset;  // Apply the offset to the position

        // Use the adjusted position for the healing range
        Collider[] hitColliders = Physics.OverlapSphere(adjustedPosition, healRange);

        foreach (Collider collider in hitColliders)
        {
            SingleSoldier soldier = collider.GetComponent<SingleSoldier>();
            if (soldier != null && soldier.CurrentHp > 0 && soldier.CurrentHp < soldier.MaxHp && soldier.CompareTag(gameObject.tag))
            {
                Heal(soldier);
            }

        }
    }

    private void Heal(SingleSoldier soldier)
    {
        //Debug.Log($"Healing {soldier.gameObject.name} for {healAmount} HP (Expected: 20)");
        soldier.HealingHp(healAmount);
        //Debug.Log($"{soldier.gameObject.name} healed to {soldier.CurrentHp} HP.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set color to green for the healing range
        //Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f); // Define the offset
        Gizmos.DrawWireSphere(transform.position, healRange); // Draw healing range sphere with offset
    }
}