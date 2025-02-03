using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSupport : Soldier
{
    public string unitTag;

    public void Start()
    {
        base.Start();
        unitTag = gameObject.tag;
        MaxHp = 100f * (1 + score / 1000);
        CurrentHp = MaxHp;
        Atk = 5 * (1 + score / 1000);
    }


    public override void OnPlaced()
    {
        base.OnPlaced();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleGateCollision(other);
    }

    private void HandleGateCollision(Collider collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && collision.gameObject.CompareTag(unitTag) && bullet.isBuff == false)
        {
            bullet.Atk += this.Atk;
            Debug.Log("Atk after buff = " + bullet.Atk);
            bullet.isBuff = true;
        }
    }

}
