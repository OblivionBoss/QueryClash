using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Healer : Unit
{
    public float HealAmount = 10f; // Amount to heal per second
    public float HealDuration = 10f; // Duration for the healing effect (in seconds)
    private Grid grid; // Reference to the grid
    private float elapsedTime = 0f;
    private HashSet<Soldier> unitsInRange = new HashSet<Soldier>();

    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        HealAmount *= 1 + (score / 1000);
        // Start the healing coroutine
        StartCoroutine(HealUnitsInRange());
    }

    private IEnumerator HealUnitsInRange()
    {
        while (elapsedTime < HealDuration)
        {
            elapsedTime += 1f;

            // Heal all units currently in range
            foreach (Soldier s in unitsInRange)
            {
                if (s != null && s.gameObject.tag == this.gameObject.tag) // Check tag match
                {
                    s.CurrentHp = Mathf.Min(s.MaxHp, s.CurrentHp + HealAmount);
                }
            }

            // Wait for 1 second before the next heal
            yield return new WaitForSeconds(1f);
        }
        Vector3Int gridPosition = grid.WorldToCell(transform.position);
        // Remove healer after healing duration
        RemoveUnit(gridPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        Soldier s = other.GetComponent<Soldier>();
        if (s != null && s != this && s.gameObject.tag == this.gameObject.tag)
        {
            unitsInRange.Add(s); // Add unit to the range list
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Soldier s = other.GetComponent<Soldier>();
        if (s != null && unitsInRange.Contains(s))
        {
            unitsInRange.Remove(s); // Remove unit when it exits the healing range
        }
    }
    public override void OnPlaced()
    {
        base.OnPlaced();
        // Optionally, play an effect or sound when placed
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to visualize the trigger collider range
        Gizmos.color = Color.green;
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
        }
    }
}
