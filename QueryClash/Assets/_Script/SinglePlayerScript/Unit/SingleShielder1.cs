using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShielder1 : SingleSoldier
{
    public float skillCooldown = 10;
    public float skillCooldownRemaining;
    public float skillDuration;
    private Animator childAnimator;

    private float Defence = 0f;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        MaxHp = 1000f * (1 + score / 1000);         // Set specific MaxHp for LeftFrontline
        spawnRate = 0f;       // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;     // Initialize bullet timer
        CurrentHp = MaxHp;    // Initialize CurrentHp to MaxHp   
        Atk = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //base.Update();
        ActivateSkill();
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

    public void ActivateSkill()
    {
        if (!isPlaced)
            return; // Do nothing if the soldier is not placed

        // Increment cooldown timer if skill is not active
        if (skillCooldownRemaining < skillCooldown && !timer.isCountingDown)
        {
            skillCooldownRemaining += Time.deltaTime;
        }

        // Activate skill if cooldown has elapsed
        if (skillCooldownRemaining >= skillCooldown && skillDuration == 0)
        {
            Debug.Log("Skill activated");
            Defence = 0.5f;
            skillDuration += Time.deltaTime; // Start counting skill duration
        }
        else if (skillDuration > 0) // Skill is active
        {
            skillDuration += Time.deltaTime;

            // Check if skill duration has ended
            if (skillDuration >= 3f)
            {
                ResetSkill();
                Debug.Log("Skill ended");
            }
        }
    }

    public void ResetSkill()
    {
        Defence = 0f; // Reset spawn rate to default
        skillCooldownRemaining = 0f; // Reset cooldown timer
        skillDuration = 0f; // Reset skill duration
    }

    public override void ReduceHp(float damage)
    {
        CurrentHp -= damage * (1-Defence);
        healthBar.fillAmount = CurrentHp / MaxHp;
        if (CurrentHp <= 0)
        {
            
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
    }

}
