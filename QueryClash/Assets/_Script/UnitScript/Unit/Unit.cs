using System;
using UnityEngine;
using FishNet.Object;

public class Unit : NetworkBehaviour
{
    private PlacementSystem placementSystem;
    public bool isPlaced = false;
    public float score;
    public bool isBase = false;

    public event Action OnDeath;

    public void Start()
    {
        placementSystem = GameObject.FindObjectOfType<PlacementSystem>();
    }

    [Server]
    public void RemoveUnit(Vector3Int gridPosition)
    {
        ClientRemoveUnit(gridPosition);

        // Destroy the GameObject
        ServerManager.Despawn(gameObject);
    }

    [ObserversRpc]
    public void ClientRemoveUnit(Vector3Int gridPosition)
    {
        // Call RemovingState logic directly
        placementSystem?.RemoveUnitAt(gridPosition);

        OnDeath?.Invoke();

        Debug.Log($"Unit at {gridPosition} has been removed.");
    }

    public virtual void OnPlaced()
    {
        isPlaced = true;
    }

    public void SetScore(float score)
    {
        this.score = score;
    }
}