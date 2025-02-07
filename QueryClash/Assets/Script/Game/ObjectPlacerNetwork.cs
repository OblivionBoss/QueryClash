using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class ObjectPlacerNetwork : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner) 
        { 
            GetComponent<ObjectPlacerNetwork>().enabled = false;
            Debug.Log("falseeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
        }
        Debug.Log("trueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnPlacedServer(GameObject newObject)
    {
        InstanceFinder.ServerManager.Spawn(newObject, null);
        Debug.Log("server place");
    }
}
