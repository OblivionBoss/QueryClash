using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Soldier : Unit
{

    public readonly SyncVar<float> MaxHp = new SyncVar<float>();
    public readonly SyncVar<float> CurrentHp = new SyncVar<float>();
    public float Atk;
    public GameObject bullet;

    public float spawnRate; // How often to spawn bullets (in seconds)
    public float bulletTimer;

    public Grid grid;

    public AudioClip bulletSpawnSound; // Assign in the inspector
    public AudioClip placedSound;
    public AudioSource audioSource;

    public Timer timer;

    public RectTransform healthRT;
    public Image healthBar;
    public TextMeshProUGUI healthText;
    

    protected Animator childAnimator;

    public new void Start()
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
        HealthBarSetup();
        CurrentHp.OnChange += HealthBarUpdate;
        MaxHp.OnChange += HealthBarUpdate; // For HealthText UI on begin client

        if (isPlaced)
        {
            audioSource.PlayOneShot(placedSound);
        }

        SetAnimator();
    }

    [Server]
    public void HandleBulletSpawning()
    {
        if (isPlaced) bulletTimer += Time.deltaTime;

        // Check if it's time to spawn a bullet
        if (bulletTimer >= spawnRate)
        {
            SpawnBullet();
            bulletTimer = 0f; // Reset timer
        }
    }

    [Server]
    public virtual void SpawnBullet()
    {
        if (bullet != null && isPlaced && !timer.isCountingDown.Value)
        {
            SpawnBulletServer();
        }
    }

    [Server]
    public virtual void SpawnBulletServer()
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

    [ObserversRpc]
    public void ClientSpawnBullet()
    {
        if (audioSource != null && bulletSpawnSound != null)
        {
            audioSource.PlayOneShot(bulletSpawnSound);
        }
    }

    [Server]
    public virtual void ReduceHp(float damage)
    {
        CurrentHp.Value = Mathf.Max(0, CurrentHp.Value - damage);
        if (CurrentHp.Value <= 0)
        {
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
    }

    [Server]
    public virtual void HealingHp(float heal)
    {
        CurrentHp.Value = Mathf.Min(MaxHp.Value, CurrentHp.Value + heal);
    }

    public void HealthBarUpdate(float prev, float next, bool asServer)
    {
        healthBar.fillAmount = CurrentHp.Value / MaxHp.Value;
        if (healthText != null)
            healthText.text = CurrentHp.Value.ToString("#0") + " / " + MaxHp.Value.ToString("#0");
    }

    public void UpdateSpawnHP(float MaxHp)
    {
        healthBar.fillAmount = 1f;
        if (healthText != null)
            healthText.text = MaxHp.ToString("#0") + " / " + MaxHp.ToString("#0");
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
        bulletTimer = 0f;
        

        if (childAnimator == null) // Reassign if null
        {
            childAnimator = GetComponentInChildren<Animator>();
        }
        if (childAnimator != null)
        {
            childAnimator.SetBool("Onplace", true);
        }
        else
        {
            Debug.LogWarning("Animator reference is null in OnPlaced!");
        }
    }

    public virtual void SetAnimator()
    {
        Debug.Log("Set animator run");
        if (this.childAnimator == null) // Reassign if null
        {
            this.childAnimator = GetComponentInChildren<Animator>();
        }

        if (this.childAnimator != null && timer != null)
        {
            StartCoroutine(WaitForCountdownAndSetAnimator());
        }
        else
        {
            Debug.LogWarning("Animator reference is null in SetAnimator!");
        }
    }

    //Coroutine to wait until countdown finishes before setting animator bool
    private IEnumerator WaitForCountdownAndSetAnimator()
    {
        while (timer == null) // Wait until timer is assigned
        {
            Debug.LogWarning("Waiting for timer to be assigned...");
            yield return null;
        }

        yield return new WaitUntil(() => !timer.isCountingDown.Value); // Wait until countdown ends

        if (childAnimator != null)
        {
            childAnimator.SetBool("Shooting", true);
            Debug.Log("Set shooting = true");
        }
        else
        {
            Debug.LogError("childAnimator is null before setting Shooting!");
        }
    }

    public virtual void HealthBarSetup()
    {
        if (ClientManager.Connection.IsHost)
        {
            if (healthRT != null) healthRT.eulerAngles = new Vector3(50f, 0f, 0f);
        }
        else
        {
            Color blue = new Color(0f, 0.2874017f, 1f);
            Color red = new Color(1f, 0.03264745f, 0f);
            Color cyan = new Color(0f, 1f, 1f);
            Color orange = new Color(1f, 0.6658927f, 0f);

            if (healthRT != null) healthRT.eulerAngles = new Vector3(50f, 180f, 0f);

            if (gameObject.CompareTag("LeftTeam"))
            {
                if (healthBar != null)
                {
                    healthBar.color = red;
                    healthBar.fillOrigin = 1;
                }
                if (healthText != null) healthText.color = orange;
            }
            else
            {
                if (healthBar != null)
                {
                    healthBar.color = blue;
                    healthBar.fillOrigin = 0;
                }
                if (healthText != null) healthText.color = cyan;
            }
        }
    }

    public virtual void FindHealthBar()
    {
        if (healthBar == null)
        {
            string healthRTPath = gameObject.CompareTag("LeftTeam") ?
                "Left Healthbar Canvas" :
                "Right Healthbar Canvas";

            healthRT = transform.Find(healthRTPath)?.GetComponent<RectTransform>();

            if (healthRT == null)
                Debug.LogError($"HealthRT UI not found for {gameObject.name} (Tag: {gameObject.tag})");

        }
        if (healthBar == null)
        {
            string healthBarPath = gameObject.CompareTag("LeftTeam") ?
                "Left Healthbar Canvas/HealthBar" :
                "Right Healthbar Canvas/HealthBar";

            healthBar = transform.Find(healthBarPath)?.GetComponent<Image>();

            if (healthBar == null)
                Debug.LogError($"HealthBar UI not found for {gameObject.name} (Tag: {gameObject.tag})");
            
        }
        if (healthText == null)
        {
            string healthTextPath = gameObject.CompareTag("LeftTeam") ?
                "Left Healthbar Canvas/HealthText" :
                "Right Healthbar Canvas/HealthText";

            healthText = transform.Find(healthTextPath)?.GetComponent<TextMeshProUGUI>();
            
            if (healthText == null)
                Debug.LogError($"HealthText UI not found for {gameObject.name} (Tag: {gameObject.tag})");
        }
    }
}