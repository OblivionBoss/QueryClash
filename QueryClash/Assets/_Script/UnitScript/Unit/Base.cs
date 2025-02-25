using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class Base : Soldier
{
    //[SyncVar] public float tempHpForNetwork;

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    this.GiveOwnership(base.Owner);
    //    if (!base.IsOwner) GetComponent<Base>().enabled = false;
    //}

    //void Start()
    //{
    //    OnPlaced();
    //    CurrentHp.Value = MaxHp;
    //    isBase = true;
    //}
    public float baseHP;

    public override void OnStartClient()
    {
        base.OnStartClient();
        timer = GameObject.FindObjectOfType<Timer>();
        OnPlaced();
        CurrentHp.Value = baseHP;
        MaxHp.Value = baseHP;
        isBase = true;
    }

    [Server]
    public override void ReduceHp(float damage)
    {
        if (!ClientManager.Connection.IsHost || !timer.isGameStart.Value) return;

        CurrentHp.Value = Mathf.Max(0, CurrentHp.Value - damage);
        if (CurrentHp.Value <= 0)
        {
            ServerManager.Despawn(gameObject);
        }
    }

    public override void HealingHp(float heal)
    {
        // Do nothing
    }

    public override void HealthBarSetup()
    {
        // Do nothing
    }

    public override void FindHealthBar()
    {
        // Do nothing, since healthBar is manually assigned
    }
}