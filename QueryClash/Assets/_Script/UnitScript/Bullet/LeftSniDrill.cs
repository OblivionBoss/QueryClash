using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftSniDrill : Bullet
{
    public override void Move()
    {
        transform.position += Direction * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        Soldier soldier = collision.gameObject.GetComponent<Soldier>();
        if (collision.gameObject.CompareTag("LeftTeam") && soldier != null && soldier.isPlaced && soldier.isBase == false)
        {
            soldier.ReduceHp(Atk.Value / 3);
        }
        else if (collision.gameObject.CompareTag("LeftTeam") && soldier != null && soldier.isPlaced && soldier.isBase == true)
        {
            Destroy(gameObject);
        }
    }
}
