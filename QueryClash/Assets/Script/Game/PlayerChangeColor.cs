using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using UnityEngine;

public class PlayerChangeColor : NetworkBehaviour
{
    
    public GameObject Body;
    public Color color;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner) { gameObject.GetComponent<PlayerControl>().enabled = false; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() { if (Input.GetKeyDown(KeyCode.C)) { ChangeColorServer(gameObject, color); } }

    [ServerRpc]
    public void ChangeColorServer(GameObject Player, Color color) { ChangeColor(Player, color); }

    [ObserversRpc]
    public void ChangeColor(GameObject Player, Color color) { Player.GetComponent<PlayerChangeColor>().Body.GetComponent<Renderer>().material.color = color; }

    // hrekslavitan
    // revrev
    // Pikachu
}
