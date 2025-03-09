using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class SingleShielder2 : SingleSoldier
{

    
    public int HitCount = 0;
    private int hitcountInterval=2;


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
                
        spawnRate = 0f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;    // Initialize bullet timer           
        MaxHp = 750f * (float)Math.Pow(1 + score / 500, 2);
        CurrentHp = MaxHp;
        Atk = 30f * (float)Math.Pow(1 + score / 500, 2);
        HealthBarUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (baseManager.gameEnd) return;
        ActiveSkill();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
    }

    public async void ActiveSkill()
    {
        if(this.HitCount >= hitcountInterval)
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
        HealthBarUpdate();
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
