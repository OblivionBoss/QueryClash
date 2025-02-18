using UnityEngine;
using FishNet.Object;

public class AssaultBullet : Bullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    [Server]
    protected override void HandleCollision(Collider collision)
    {
        Soldier soldier = collision.gameObject.GetComponent<Soldier>();
        if (collision.gameObject.CompareTag(enemyTag) && soldier != null && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk.Value);
            ServerManager.Despawn(gameObject);
        }
    }
}
