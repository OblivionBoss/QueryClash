using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCannon : SingleSoldier
{
    void Start()
    {
        base.Start();
        MaxHp = 200f * (1 + score / 1000);         // Set specific MaxHp for LeftFrontline
        spawnRate = 2.5f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 30 * (1 + score / 1000);

    }
    void Update()
    {
        //base.Update();
        HandleBulletSpawning();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
    }
}
