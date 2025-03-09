using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCannon : SingleSoldier
{
    void Start()
    {
        base.Start();
        MaxHp = 160f * Mathf.Pow(1 + score / 500f, 2);         // Set specific MaxHp for LeftFrontline
        spawnRate = 2.5f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 20 * Mathf.Pow(1 + score / 500f, 2);
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
