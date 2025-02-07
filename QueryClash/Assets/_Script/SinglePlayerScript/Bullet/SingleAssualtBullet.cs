
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAssaultBullet : SingleBullet
{
    // Start is called before the first frame update
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        SingleSoldier soldier = collision.gameObject.GetComponent<SingleSoldier>();
        if (collision.gameObject.CompareTag(enemyTag) && soldier != null && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk);
            Destroy(gameObject);
        }
    }
}
