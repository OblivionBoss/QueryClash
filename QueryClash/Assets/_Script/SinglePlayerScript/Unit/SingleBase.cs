using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SingleBase : SingleSoldier
{
    public float baseHP;

    void Start()
    {
        OnPlaced();
        MaxHp = baseHP;
        CurrentHp = baseHP;
        isBase = true;
        UpdateSpawnHP(MaxHp);
    }

    public override void ReduceHp(float damage)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - damage);
        HealthBarUpdate();
        if (CurrentHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void HealingHp(float heal)
    {
        
    }

    public override void HealthBarSetup()
    {
        // Do nothing
    }

    public override void FindHealthBar()
    {
        // Do nothing, since healthBar is manually assigned
    }

    public override void SetHealthCanvas()
    {
        //Do not set Health canvas on Base object.
    }
}
