using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Unit
{
    public float healAmount = 10f;    // Amount healed per second
    public float healDuration = 10f;  // Total healing duration
    public float healInterval = 1f;   // Heal every second
    public float healRange = 1.2f;      // Healing radius

    private bool isHealing = false;
    //public Timer timer;

    public Grid grid;
    public Timer timer;
    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        timer = GameObject.FindObjectOfType<Timer>();
        //if (audioSource == null)
        //{
        //    audioSource = GetComponent<AudioSource>();
        //    if (audioSource == null)
        //    {
        //        audioSource = gameObject.AddComponent<AudioSource>();
        //    }
        //}
    }

    public override void OnPlaced()
    {
        base.OnPlaced();

        StartCoroutine(HealOverTime());

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
            if (soldier != null && soldier.CurrentHp.Value > 0 && soldier.CurrentHp.Value < soldier.MaxHp)
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