using UnityEngine;

public class SingleBase : SingleSoldier
{
    public float baseHP;
    [SerializeField]
    private GameObject explodeFX;
    [SerializeField]
    private AudioClip explodeSound;
    

    void Start()
    {
        //if (audioSource == null)
        //{
        //    audioSource = GetComponent<AudioSource>();
        //    if (audioSource == null)
        //    {
        //        audioSource = gameObject.AddComponent<AudioSource>();
        //    }
        //}
        OnPlaced();
        MaxHp = baseHP;
        CurrentHp = baseHP;
        isBase = true;
        HealthBarUpdate();
    }

    public  void Update()
    {
        
    }
    public override void ReduceHp(float damage)
    {
        if (baseManager.gameEnd) return;
        CurrentHp = Mathf.Max(0, CurrentHp - damage);
        HealthBarUpdate();
        if (CurrentHp <= 0)
        {
            //Destroy(gameObject);
            explodeFX.SetActive(true);
            audioSource.PlayOneShot(explodeSound);
            if (baseManager.gameEnd) return;
        }
        
    }

    public override void HealingHp(float heal)
    {
        
    }

    public override void HealthBarSetup()
    {
        // Do nothing
    }

    public override void FindHealthBar()
    {
        // Do nothing, since healthBar is manually assigned
    }

    public override void SetHealthCanvas()
    {
        //Do not set Health canvas on Base object.
    }

    public override void SetAnimator()
    {
        
    }
}
