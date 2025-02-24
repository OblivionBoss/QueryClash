using UnityEngine;
using FishNet.Object;

public class BuffSupport : Soldier
{
    public string unitTag;
    public GameObject specialEffect;
    public new void Start()
    {
        base.Start();
        unitTag = gameObject.tag;
        MaxHp.Value = 100f * (1 + score.Value / 1000);
        CurrentHp.Value = MaxHp.Value;
        Atk = 5 * (1 + score.Value / 1000);
    }

    public override void OnPlaced()
    {
        base.OnPlaced();
        if (specialEffect != null)
        {
            specialEffect.SetActive(true);
        }
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (ClientManager.Connection.IsHost)
            HandleGateCollision(other);
    }

    [Server]
    private void HandleGateCollision(Collider collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && collision.gameObject.CompareTag(unitTag) && !bullet.isBuff.Value)
        {
            bullet.Atk.Value += Atk;
            Debug.Log("Atk after buff = " + bullet.Atk.Value);
            bullet.isBuff.Value = true;
        }
    }
}