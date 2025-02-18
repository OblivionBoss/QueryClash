using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class ObjectPlacer : NetworkBehaviour
{
    private readonly SyncList<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position, float score)
    {
        PlaceObjectServer(prefab, position, score);

        // {return placedGameObjects.Count} For deal with network delay only because placedGameObjects does not update instant after call function PlaceObjectServer
        return placedGameObjects.Count;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaceObjectServer(GameObject prefab, Vector3 position, float score)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        ServerManager.Spawn(newObject, null);
        placedGameObjects.Add(newObject);

        // Check if the placed object has a CapsuleUnit script
        Unit unit = newObject.GetComponent<Unit>();
        if (unit != null)
        {
            // Call the OnPlaced method of the CapsuleUnit
            unit.OnPlaced();
            unit.SetScore(score);
        }
    }

    internal void RemoveOBjectAt(int gameObjectIndex)
    {
        RemoveOBjectAtNetwork(gameObjectIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    internal void RemoveOBjectAtNetwork(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            Debug.LogWarning($"Can not remove unit at index = {gameObjectIndex}");
            return;
        }
        ServerManager.Despawn(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }

    public GameObject GetPlacedObject(int index) // For EnemySpawner
    {
        if (index < 0 || index >= placedGameObjects.Count)
        {
            Debug.LogWarning($"Index {index} is out of bounds.");
            return null;
        }
        return placedGameObjects[index];
    }
}