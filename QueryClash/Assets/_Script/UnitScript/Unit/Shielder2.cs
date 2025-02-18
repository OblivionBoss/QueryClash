using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class Shielder2 : Soldier
{
    private Animator childAnimator;
    public readonly SyncVar<int> HitCount = new SyncVar<int>(0);

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();
        MaxHp.Value = 750f * (1 + score / 1000);  // Set specific MaxHp for LeftFrontline
        spawnRate = 0f;                     // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 20f * (1 + score / 1000);
    }

    [Server]
    void Update()
    {
        if (ClientManager.Connection.IsHost)
            ActiveSkill();
    }

    [Server]
    public void ActiveSkill()
    {
        if (HitCount.Value >= 10)
        {
            SpawnBullet();
            HitCount.Value = 0;
        }
    }

    [Server]
    public override void ReduceHp(float damage)
    {
        CurrentHp.Value = Mathf.Max(0, CurrentHp.Value - damage);
        HitCount.Value++;
        if (CurrentHp.Value <= 0)
        {
            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            // Remove the unit from the PlacementSystem
            RemoveUnit(gridPosition);
        }
    }
}