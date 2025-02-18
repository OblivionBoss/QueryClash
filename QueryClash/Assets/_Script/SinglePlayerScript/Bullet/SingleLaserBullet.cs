using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLaserBullet : SingleBullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        SingleSoldier soldier = collision.gameObject.GetComponent<SingleSoldier>();

        if (soldier != null && collision.gameObject.CompareTag(enemyTag) && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk); // Damage the soldier

            if (soldier.isBase) // If it's a base, destroy the bullet
            {
                Destroy(gameObject);
            }
            else
            {
                this.Atk *= 2f / 3f; // Reduce attack power for next hits
            }
        }
    }
}
