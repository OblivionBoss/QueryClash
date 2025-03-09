using UnityEngine;
using FishNet.Object;

public class Cannon : Soldier
{
    public new void Start()
    {
        base.Start();

        float maxhp = 160f * (Mathf.Pow(1 + score.Value / 500f, 2));
        UpdateSpawnHP(maxhp);

        MaxHp.Value = maxhp;  // Set specific MaxHp for LeftFrontline
        spawnRate = 2.5f;                   // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 20 * (Mathf.Pow(1 + score.Value / 500f, 2));
    }

    [Server]
    void Update()
    {
        if (ClientManager.Connection.IsHost && timer.isGameStart.Value)
            HandleBulletSpawning();
    }
}