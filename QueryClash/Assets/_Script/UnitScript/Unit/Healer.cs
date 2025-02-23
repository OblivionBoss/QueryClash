using FishNet.Object;
using System.Collections;
using UnityEngine;

public class Healer : Unit
{
    public float healAmount = 10f;      // Amount healed per second
    public float healDuration = 10f;    // Total healing duration
    public float healInterval = 1f;     // Heal every second
    public float healRange = 1.2f;      // Healing radius

    private bool isHealing = false;

    public Grid grid;
    public Timer timer;

    public GameObject specialEffect;
    public AudioClip healingSound;
    public AudioSource healingAudioSource;

    public new void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        timer = GameObject.FindObjectOfType<Timer>();

        if (healingSound != null && isPlaced) PlayHealingSound();
    }

    [Server]
    public void Update()
    {
        if (!ClientManager.Connection.IsHost) return;

        if (isPlaced && !timer.isCountingDown.Value)
        {
            StartCoroutine(HealOverTime());
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

    public override void OnPlaced()
    {
        base.OnPlaced();
        //StartCoroutine(HealOverTime()); // Comment this line if there's a issue.
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
            Soldier soldier = collider.GetComponent<Soldier>();
            if (soldier != null && soldier.CurrentHp.Value > 0 && soldier.CurrentHp.Value < soldier.MaxHp.Value)
            {
                Heal(soldier);
            }
        }
    }

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