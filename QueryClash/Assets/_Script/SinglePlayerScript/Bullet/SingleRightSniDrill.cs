using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRightSniDrill : SingleBullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        SingleSoldier soldier = collision.gameObject.GetComponent<SingleSoldier>();
        if (collision.gameObject.CompareTag("LeftTeam") && soldier != null && soldier.isPlaced && soldier.isBase == false)
        {
            soldier.ReduceHp(Atk/3);
            
        }else if(collision.gameObject.CompareTag("LeftTeam") && soldier != null && soldier.isPlaced && soldier.isBase == true)
        {
            Destroy(gameObject);
        }
    }
}
