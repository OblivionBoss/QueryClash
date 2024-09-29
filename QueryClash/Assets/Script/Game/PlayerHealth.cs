using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerHealth : NetworkBehaviour
{
    private readonly SyncVar<int> Health = new SyncVar<int>(100);
    public override void OnStartClient() { base.OnStartClient(); if (!base.IsOwner) { GetComponent<PlayerSpawnObject>().enabled = false; } }

    private void Update() { if (Input.GetKeyDown(KeyCode.H)) { UpdateHealth(this, -1); } }

    [ServerRpc]
    public void UpdateHealth(PlayerHealth Script, int Change) { Health.Value += Change; }
}
