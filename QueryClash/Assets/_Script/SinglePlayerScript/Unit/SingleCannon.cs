using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCannon : SingleSoldier
{
    void Start()
    {
        base.Start();
                // Set specific MaxHp for LeftFrontline
        spawnRate = 2.5f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        
        MaxHp = 160f * (float)Math.Pow(1 + score / 500, 2);
        CurrentHp = MaxHp;
        Atk = 15 * (float)Math.Pow(1 + score / 500, 2);
        HealthBarUpdate();
    }
    public void Update()
    {
        if (baseManager.gameEnd) return;
        HandleBulletSpawning();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
    }
}
