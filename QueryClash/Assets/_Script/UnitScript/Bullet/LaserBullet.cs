using UnityEngine;
using FishNet.Object;

public class LaserBullet : Bullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    [Server]
    protected override void HandleCollision(Collider collision)
    {
        Soldier soldier = collision.gameObject.GetComponent<Soldier>();

        if (soldier != null && collision.gameObject.CompareTag(enemyTag) && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk.Value); // Damage the soldier

            if (soldier.isBase) // If it's a base, destroy the bullet
            {
                Destroy(gameObject);
            }
            else
            {
                Atk.Value *= 2f / 3f; // Reduce attack power for next hits
            }
        }
    }
}