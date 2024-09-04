using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Transporting.Tugboat;
using FishNet.Transporting;

public class ConnectionStarter : MonoBehaviour
{
    private Tugboat _tugboat;

    private void OnEnable() { FishNet.InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState; } // Subscribe
    private void OnDisable() { FishNet.InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState; } // Unsubscribe

    private void OnClientConnectionState(FishNet.Transporting.ClientConnectionStateArgs args) 
    { if (args.ConnectionState == LocalConnectionState.Stopping) UnityEditor.EditorApplication.isPlaying = false; } // If host abort connections, stop playing

    // Start is called before the first frame update
    private void Start()
    {
        if (TryGetComponent(out Tugboat _t)) { _tugboat = _t; }
        else { Debug.LogError("Can't get tugboat bro...", this); return; }
        
        if (ParrelSync.ClonesManager.IsClone()) { _tugboat.StartConnection(false); }
        else { _tugboat.StartConnection(true); _tugboat.StartConnection(false); }
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
