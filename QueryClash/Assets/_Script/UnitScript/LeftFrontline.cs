using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftFrontline : Soldier
{
    void Start()
    {
        base.Start();
        MaxHp = 100f;         // Set specific MaxHp for LeftFrontline
        spawnRate = 1f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 10;
        
    }

    void Update()
    {
        base.Update();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();

        //gameObject.layer = 3;
    }

}
