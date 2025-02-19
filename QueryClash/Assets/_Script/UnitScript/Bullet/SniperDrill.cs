using UnityEngine;
using FishNet.Object;

public class SniperDrill : Bullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    [Server]
    protected override void HandleCollision(Collider collision)
    {
        if (!collision.gameObject.CompareTag(enemyTag)) return;

        Soldier soldier = collision.gameObject.GetComponent<Soldier>();
        if (soldier == null || !soldier.isPlaced) return;

        if (soldier.isBase)
        {
            Destroy(gameObject); // Destroy bullet if it hits a base
        }
        else
        {
            soldier.ReduceHp(Atk.Value / 3); // Deal damage to normal soldiers
        }
    }
}