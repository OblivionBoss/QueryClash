using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder2 : Soldier
{

    private Animator childAnimator;
    public int HitCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        MaxHp = 750f;         // Set specific MaxHp for LeftFrontline
        spawnRate = 0f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        ActiveSkill();
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
        //if (childAnimator == null) // Reassign if null
        //{
        //    childAnimator = GetComponentInChildren<Animator>();
        //}
        //if (childAnimator != null)
        //{
        //    childAnimator.SetBool("Shooting", true);
        //    Debug.Log("Set shooting = true");
        //}
        //else
        //{
        //    Debug.LogWarning("Animator reference is null in OnPlaced!");
        //}

    }

    public void ActiveSkill()
    {
        if(this.HitCount >= 10)
        {
            SpawnBullet();
            HitCount = 0;
        }
    }

    public override void ReduceHp(float damage)
    {
        CurrentHp -= damage;
        HitCount++;
        if (CurrentHp <= 0)
        {
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
    }
}
