using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Soldier
{

    
    // Start is called before the first frame update
    void Start()
    {
        OnPlaced();
        CurrentHp = MaxHp;
        isBase = true;
    }

    public override void ReduceHp(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
