using UnityEngine;
using FishNet.Object;

public class Sniper2 : Soldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;
    
    private bool skillUsing;
    public GameObject specialBullet;

    public new void Start()
    {
        base.Start();

        float maxhp = 100f * Mathf.Pow(1 + score.Value / 500f, 2);
        UpdateSpawnHP(maxhp);

        MaxHp.Value = maxhp;   // Set specific MaxHp for LeftFrontline
        spawnRate = 2f;                     // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 20 * Mathf.Pow(1 + score.Value / 500f, 2); 
    }

    [Server]
    void Update()
    {
        if (!ClientManager.Connection.IsHost || !timer.isGameStart.Value) return;
        HandleBulletSpawning();
        ActivateSkill();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
        skillUsing = false;
        
    }

    [Server]
    public override void SpawnBulletServer()
    {
        if (skillUsing)
        {
            SpawnSniperBulletServer(specialBullet);
            ResetSkill();
        }
        else
        {
            SpawnSniperBulletServer(bullet);
        }
    }

    [Server]
    public void SpawnSniperBulletServer(GameObject bulletType)
    {
        GameObject spawnedBullet = Instantiate(bulletType, transform.position, transform.rotation);
        ServerManager.Spawn(spawnedBullet, null);

        Bullet bulletComponent = spawnedBullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            // Determine direction and dead zone based on the soldier's tag
            if (gameObject.CompareTag("LeftTeam"))
            {
                bulletComponent.Initialize(Atk, 100f, Vector3.right, "RightTeam", "LeftTeam"); // Bullets move right
            }
            else if (gameObject.CompareTag("RightTeam"))
            {
                bulletComponent.Initialize(Atk, -100f, Vector3.left, "LeftTeam", "RightTeam"); // Bullets move left
            }
        }
        else
        {
            Debug.LogWarning("Spawned object does not have a Bullet component!");
        }
        ClientSpawnBullet();
    }

    [Server]
    public void ActivateSkill()
    {
        if (!isPlaced) return; // Do nothing if the soldier is not placed

        // Increment cooldown timer if skill is not active
        if (skillCooldownRemaining < skillCooldown && !timer.isCountingDown.Value)
        {
            skillCooldownRemaining += Time.deltaTime;
        }

        // Activate skill if cooldown has elapsed
        if (skillCooldownRemaining >= skillCooldown)
        {
            Debug.Log("Skill activated");
            skillUsing = true;
        }
    }

    [Server]
    public void ResetSkill()
    {
        skillCooldownRemaining = 0f; // Reset cooldown timer
        skillUsing = false;
    }
}