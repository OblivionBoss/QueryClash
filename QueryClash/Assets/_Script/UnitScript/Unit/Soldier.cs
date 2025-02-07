using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.UI;

public class Soldier : Unit
{

    public float MaxHp;
    public readonly SyncVar<float> CurrentHp = new();
    public float Atk;
    public GameObject bullet;

    public float spawnRate; // How often to spawn bullets (in seconds)
    public float bulletTimer;

    public Grid grid;

    public AudioClip bulletSpawnSound; // Assign in the inspector
    public AudioSource audioSource;

    public Timer timer;

    public Image healthBar;

    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        timer = GameObject.FindObjectOfType<Timer>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        FindHealthBar();
    }

    public void HandleBulletSpawning()
    {
        if (isPlaced)
        {
            bulletTimer += Time.deltaTime;
        }

        // Check if it's time to spawn a bullet
        if (bulletTimer >= spawnRate)
        {
            SpawnBullet();
            bulletTimer = 0f; // Reset timer
        }
    }

    public virtual void SpawnBullet()
    {
        if (bullet != null && isPlaced && timer.isCountingDown == false)
        {
            SpawnBulletServer();
        }
    }

    [Server]
    public void SpawnBulletServer()
    {
        if (ClientManager.Connection.IsHost)
        {
            GameObject spawnedBullet = Instantiate(bullet, transform.position, transform.rotation);
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
            ClientSpawnBullet();
        }
    }

    [ObserversRpc]
    public void ClientSpawnBullet()
    {
        if (audioSource != null && bulletSpawnSound != null)
        {
            audioSource.PlayOneShot(bulletSpawnSound);
        }
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
    }

    [Server]
    public virtual void ReduceHp(float damage)
    {
        CurrentHp.Value -= damage;
        if (CurrentHp.Value <= 0)
        {
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
        ClientHealthBarUpdate();
    }

    [ObserversRpc]
    public void ClientHealthBarUpdate()
    {
        healthBar.fillAmount = CurrentHp.Value / MaxHp;
    }

    [Server]
    public virtual void HealingHp(float heal)
    {
        //this.CurrentHp.Value += heal;
        CurrentHp.Value = Mathf.Min(MaxHp, CurrentHp.Value + heal);
        //if (CurrentHp.Value >= MaxHp)
        //{
        //    CurrentHp.Value = MaxHp;
        //}
        healthBar.fillAmount = CurrentHp.Value / MaxHp;
    }

    public virtual void FindHealthBar()
    {
        if (healthBar == null)
        {
            string healthBarPath = gameObject.CompareTag("LeftTeam") ?
                "Left Healthbar Canvas/HealthBar" :
                "Right Healthbar Canvas/HealthBar";

            healthBar = transform.Find(healthBarPath)?.GetComponent<Image>();

            if (healthBar == null)
            {
                Debug.LogError($"HealthBar UI not found for {gameObject.name} (Tag: {gameObject.tag})");
            }
        }
    }
}
