using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object;

public class PlayerSpawnObject : NetworkBehaviour
{

    public GameObject ObjectToSpawn;
    [HideInInspector] public GameObject spawnedObject;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner) { GetComponent<PlayerSpawnObject>().enabled = false; }
    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update() { 
        if (Input.GetKeyDown(KeyCode.B)) {
            if (spawnedObject == null) { SpawnedObject(ObjectToSpawn, transform, this); }
            else { DespawnedObject(spawnedObject); }
        } 
     }

    [ServerRpc]
    public void SpawnedObject(GameObject Obj, Transform Player, PlayerSpawnObject Script) {
        GameObject Spawned = Instantiate(Obj, Player.position + Player.forward, Quaternion.identity);
        ServerManager.Spawn(Spawned);
        SetSpawnedObject(Spawned, Script);
    }

    [ObserversRpc]
    public void SetSpawnedObject(GameObject Spawned, PlayerSpawnObject Script) { Script.spawnedObject = Spawned; }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnedObject(GameObject Obj) { ServerManager.Despawn(Obj); }
}
