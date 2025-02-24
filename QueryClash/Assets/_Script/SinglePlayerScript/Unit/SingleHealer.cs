using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHealer : SingleUnit
{
    public float healAmount = 20f;    // Amount healed per second
    public float healDuration = 10f;  // Total healing duration
    public float healInterval = 2f;   // Heal every second
    public float healRange = 1.2f;      // Healing radius

    private bool isHealing = false;
   
    public AudioClip HealingSound; // Assign in the inspector
    public AudioSource healingAudioSource;

    public Grid grid;
    public SingleTimer timer;

    public GameObject specialEffect;
    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        timer = GameObject.FindObjectOfType<SingleTimer>();

        

        if (HealingSound != null && isPlaced)
        {
            PlayHealingSound();
        }
    }

    public void Update()
    {
        if (isPlaced && !timer.isCountingDown)
        {
            //StartCoroutine(HealOverTime());
            if (specialEffect == null)
            {
                specialEffect = transform.Find("Healing FX")?.gameObject;
            }
            if (specialEffect != null)
            {
                specialEffect.SetActive(true); // Show the child object
            }
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

        if (!isHealing) 
        {
            StartCoroutine(HealOverTime());
        }

        if (HealingSound != null)
        {
            PlayHealingSound();
        }
    }


    private IEnumerator HealOverTime()
    {
        isHealing = true;
        
        float elapsedTime = 0f;

        while (elapsedTime < healDuration)
        {
            HealNearbyUnits();
            elapsedTime += healInterval;
            yield return new WaitForSeconds(healInterval);
        }

        isHealing = false;

        Debug.Log($"{gameObject.name} finished healing and has been destroyed.");
        Vector3Int gridPosition = grid.WorldToCell(transform.position);
        RemoveUnit(gridPosition);
        StopHealingSound();

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
            if (soldier != null && soldier.CurrentHp > 0 && soldier.CurrentHp < soldier.MaxHp)
            {
                Heal(soldier);
            }
        }
    }

    private void Heal(SingleSoldier soldier)
    {
        soldier.HealingHp(healAmount);
        
        Debug.Log($"{soldier.gameObject.name} healed to {soldier.CurrentHp} HP.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set color to green for the healing range
        //Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f); // Define the offset
        Gizmos.DrawWireSphere(transform.position , healRange); // Draw healing range sphere with offset
    }
}