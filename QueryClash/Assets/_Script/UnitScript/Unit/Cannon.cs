using UnityEngine;
using FishNet.Object;

public class Cannon : Soldier
{
   

    public new void Start()
    {
        base.Start();
        MaxHp.Value = 200f * (1 + score / 1000);  // Set specific MaxHp for LeftFrontline
        spawnRate = 2.5f;                   // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 30 * (1 + score / 1000);

    }

    [Server]
    void Update()
    {
        if (ClientManager.Connection.IsHost)
            HandleBulletSpawning();
    }

    
}
