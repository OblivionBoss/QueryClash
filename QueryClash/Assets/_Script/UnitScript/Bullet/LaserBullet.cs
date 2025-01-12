using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBullet : Bullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        Soldier soldier = collision.gameObject.GetComponent<Soldier>();
        if (collision.gameObject.CompareTag(enemyTag) && soldier != null && soldier.isPlaced && soldier.isBase == false)
        {
            soldier.ReduceHp(Atk);
        }
        else if (collision.gameObject.CompareTag(enemyTag) && soldier != null && soldier.isPlaced && soldier.isBase == true)
        {
            soldier.ReduceHp(Atk);
            Destroy(gameObject);
        }
    }
}
