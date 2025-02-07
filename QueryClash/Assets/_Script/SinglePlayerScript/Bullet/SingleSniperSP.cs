using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSniperSP : SingleBullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        SingleBase enemyBase = collision.gameObject.GetComponent<SingleBase>();
        if (collision.gameObject.CompareTag(enemyTag) && enemyBase != null && enemyBase.isPlaced)
        {
            enemyBase.ReduceHp(Atk);
            Destroy(gameObject);
        }
    }
}
