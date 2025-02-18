using UnityEngine;
using FishNet.Object;

public class SniperSP : Bullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    [Server]
    protected override void HandleCollision(Collider collision)
    {
        Base enemyBase = collision.gameObject.GetComponent<Base>();
        if (collision.gameObject.CompareTag(enemyTag) && enemyBase != null && enemyBase.isPlaced)
        {
            enemyBase.ReduceHp(Atk.Value);
            ServerManager.Despawn(gameObject);
        }
    }
}