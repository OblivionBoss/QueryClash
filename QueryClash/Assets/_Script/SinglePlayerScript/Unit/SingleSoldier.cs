using FishNet.Managing.Client;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SingleSoldier : SingleUnit
{
    public float MaxHp;
    public float CurrentHp;
    public float Atk;
    public GameObject bullet;

    public float spawnRate; // How often to spawn bullets (in seconds)
    public float bulletTimer;

    public Grid grid;

    public AudioClip bulletSpawnSound; // Assign in the inspector
    public AudioClip placedSound;
    public AudioSource audioSource;

    public SingleTimer timer;

    public GameObject healthCanvas;
    public RectTransform healthRT;
    public Image healthBar;
    public TextMeshProUGUI healthText;

    protected Animator childAnimator;
    public void Start()
    {
        base.Start();
        grid = GameObject.FindObjectOfType<Grid>();
        timer = GameObject.FindObjectOfType<SingleTimer>();
        if (timer == null)
        {
            //Debug.LogError("Timer not found in the scene!");
        }
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
        if (isPlaced)
        {
            audioSource.PlayOneShot(placedSound);
        }
        
        SetAnimator();
    }

    public void HandleBulletSpawning()
    {
        if (isPlaced && !timer.isCountingDown)
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
        if (audioSource != null && bulletSpawnSound != null && timer.isCountingDown == false)
        {
            audioSource.PlayOneShot(bulletSpawnSound);
        }

        if (bullet != null && isPlaced && timer.isCountingDown == false)
        {
            GameObject spawnedBullet = Instantiate(bullet, transform.position, transform.rotation);

            SingleBullet bulletComponent = spawnedBullet.GetComponent<SingleBullet>();
            if (bulletComponent != null)
            {
                // Determine direction and dead zone based on the soldier's tag
                if (gameObject.CompareTag("LeftTeam"))
                {
                    bulletComponent.Initialize(Atk, 15f, Vector3.right, "RightTeam", "LeftTeam"); // Bullets move right
                }
                else if (gameObject.CompareTag("RightTeam"))
                {
                    bulletComponent.Initialize(Atk, -15f, Vector3.left, "LeftTeam", "RightTeam"); // Bullets move left
                }

                //Debug.Log($"Spawned bullet from {gameObject.tag} with Atk: {bulletComponent.Atk}");
            }
            else
            {
                Debug.LogWarning("Spawned object does not have a Bullet component!");
            }
        }
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
        bulletTimer = 0f;
        SetHealthCanvas();
        
        if (childAnimator == null) // Reassign if null
        {
            childAnimator = GetComponentInChildren<Animator>();
        }
        if (childAnimator != null)
        {
            childAnimator.SetBool("Onplace", true);
            Debug.Log("Set Onplace = true");
        }
        else
        {
            Debug.LogWarning("Animator reference is null in OnPlaced!");
        }
    }

    public virtual void ReduceHp(float damage)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - damage);
        HealthBarUpdate();
        if (CurrentHp <= 0)
        {
            //if (grid == null)
            //{
            //    Debug.LogError("Grid is not assigned to the unit!");
            //    return;
            //}
            // Get the grid position of the unit
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
    }

    public virtual void HealingHp(float heal)
    {
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + heal);
        HealthBarUpdate();
    }

    public void HealthBarUpdate()
    {
        healthBar.fillAmount = CurrentHp / MaxHp;
        if (healthText != null)
            healthText.text = CurrentHp.ToString("#0") + " / " + MaxHp.ToString("#0");
    }

    public void UpdateSpawnHP(float MaxHp)
    {
        healthBar.fillAmount = 1f;
        if (healthText != null)
            healthText.text = MaxHp.ToString("#0") + " / " + MaxHp.ToString("#0");
    }

    public virtual void HealthBarSetup()
    {
        if (healthRT != null) healthRT.eulerAngles = new Vector3(50f, 0f, 0f);
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

    public virtual void SetHealthCanvas()
    {
        healthCanvas.SetActive(true);
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

        yield return new WaitUntil(() => !timer.isCountingDown); // Wait until countdown ends

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
}