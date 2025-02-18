using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSniperDrill : SingleBullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        if (!collision.gameObject.CompareTag(enemyTag)) return;

        SingleSoldier soldier = collision.gameObject.GetComponent<SingleSoldier>();
        if (soldier == null || !soldier.isPlaced) return;

        if (soldier.isBase)
        {
            Destroy(gameObject); // Destroy bullet if it hits a base
        }
        else
        {
            soldier.ReduceHp(Atk / 3); // Deal damage to normal soldiers
        }
    }

}
