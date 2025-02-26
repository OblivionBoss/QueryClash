using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SingleShielder2 : SingleSoldier
{

    
    public int HitCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        MaxHp = 750f * (1 + score / 1000);         // Set specific MaxHp for LeftFrontline
        spawnRate = 0f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 30f * (1 + score / 1000);
    }

    // Update is called once per frame
    void Update()
    {
        ActiveSkill();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
    }

    public async void ActiveSkill()
    {
        if(this.HitCount >= 10)
        {
            this.childAnimator = GetComponentInChildren<Animator>();
            childAnimator.SetBool("Shooting", true);
            SpawnBullet();
            HitCount = 0;
            await Task.Delay(500);
            childAnimator.SetBool("Shooting", false);
        }
    }

    public override void ReduceHp(float damage)
    {
        CurrentHp -= damage;
        healthBar.fillAmount = CurrentHp / MaxHp;
        HitCount++;
        if (CurrentHp <= 0)
        {
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
    }

    public override void SetAnimator()
    {
        // Do not need to set Animator param in this unit 
    }
}
