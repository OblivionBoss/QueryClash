using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBuffSupport : SingleSoldier
{
    public string unitTag;
    public GameObject specialEffect;
    public void Start()
    {
        base.Start();
        unitTag = gameObject.tag;
        MaxHp = 300f * (float)Math.Pow(1 + score / 500, 2);
        CurrentHp = MaxHp;
        Atk = 5 * (float)Math.Pow(1 + score / 500,2);
        HealthBarUpdate();
    }

    private void Update()
    {
        if (baseManager.gameEnd) return;
    }
    public override void OnPlaced()
    {
        base.OnPlaced();
        
        if (specialEffect != null)
        {
            specialEffect.SetActive(true); // Show the child object
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleGateCollision(other);
    }

    private void HandleGateCollision(Collider collision)
    {
        SingleBullet bullet = collision.GetComponent<SingleBullet>();
        if (bullet != null && collision.gameObject.CompareTag(unitTag) && bullet.isBuff == false)
        {
            bullet.Atk += this.Atk;
            Debug.Log("Atk after buff = " + bullet.Atk);
            bullet.isBuff = true;
        }
    }

}
