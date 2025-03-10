using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightSniperSP : Bullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        Base enemyBase = collision.gameObject.GetComponent<Base>();
        if (collision.gameObject.CompareTag("LeftTeam") && enemyBase != null && enemyBase.isPlaced)
        {
            enemyBase.ReduceHp(Atk.Value);
            Destroy(gameObject);
        }
    }
}
