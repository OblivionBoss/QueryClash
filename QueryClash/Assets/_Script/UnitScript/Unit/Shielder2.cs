using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Threading.Tasks;
using System.Collections;

public class Shielder2 : Soldier
{
    
    public readonly SyncVar<int> HitCount = new SyncVar<int>(0);

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();

        float maxhp = 750f * Mathf.Pow(1 + score.Value / 500f, 2);
        UpdateSpawnHP(maxhp);

        MaxHp.Value = maxhp;  // Set specific MaxHp for LeftFrontline
        spawnRate = 0f;                     // Set specific spawn rate How often to spawn bullets (in seconds)
        bulletTimer = 0f;                   // Initialize bullet timer
        CurrentHp.Value = MaxHp.Value;            // Initialize CurrentHp to MaxHp   
        Atk = 20f * Mathf.Pow(1 + score.Value / 500f, 2);
    }

    [Server]
    void Update()
    {
        if (ClientManager.Connection.IsHost && timer.isGameStart.Value)
            ActiveSkill();
    }

    [Server]
    public void ActiveSkill()
    {
        if(HitCount.Value >= 5)
        {
            SpawnBullet();
            HitCount.Value = 0;
            PlayAnimationClient();
        }
    }

    [ObserversRpc]
    public void PlayAnimationClient()
    {
        childAnimator = GetComponentInChildren<Animator>();
        childAnimator.SetBool("Shooting", true);
        StartCoroutine(DelayStopAnimation(0.5f));
    }

    private IEnumerator DelayStopAnimation(float delay)
    {
        yield return new WaitForSeconds(delay); // Waits for 3 seconds
        childAnimator.SetBool("Shooting", false);
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

    public override void SetAnimator()
    {
        // do nothing
    }
}