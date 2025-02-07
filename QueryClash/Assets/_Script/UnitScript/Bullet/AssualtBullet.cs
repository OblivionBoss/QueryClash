using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class AssaultBullet : Bullet
{
    // Start is called before the first frame update
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        Soldier soldier = collision.gameObject.GetComponent<Soldier>();
        if (collision.gameObject.CompareTag(enemyTag) && soldier != null && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk);
            ServerManager.Despawn(gameObject);
        }
    }
}
