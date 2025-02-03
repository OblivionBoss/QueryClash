using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEditor.Rendering;
using FishNet.Object;
using FishNet;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class ObjectPlacer : NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();
    public ObjectPlacerNetwork objectPlacerNetwork;

    public int PlaceObject(GameObject prefab, Vector3 position, float score)
    {
        /*
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        ServerManager.Spawn(newObject, null);
        //objectPlacerNetwork.OnPlacedServer(newObject);
        //InstanceFinder.ServerManager.Spawn(newObject, null);
        //newObject.transform.position = position;
        placedGameObjects.Add(newObject);

        // Check if the placed object has a CapsuleUnit script
        Unit unit = newObject.GetComponent<Unit>();
        if (unit != null)
        {
            // Call the OnPlaced method of the CapsuleUnit
            unit.OnPlaced();
            unit.SetScore(score);
        }
        */
        PlaceObjectServer(prefab, position, score);
        //Debug.LogWarning("placedGameObjectsssssssssssssssssssssssssssssss = " + (placedGameObjects.Count - 1));
        return placedGameObjects.Count - 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaceObjectServer(GameObject prefab, Vector3 position, float score)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        ServerManager.Spawn(newObject, null);
        //objectPlacerNetwork.OnPlacedServer(newObject);
        //InstanceFinder.ServerManager.Spawn(newObject, null);
        //newObject.transform.position = position;
        placedGameObjects.Add(newObject);

        // Check if the placed object has a CapsuleUnit script
        Unit unit = newObject.GetComponent<Unit>();
        if (unit != null)
        {
            // Call the OnPlaced method of the CapsuleUnit
            unit.OnPlaced();
            unit.SetScore(score);
        }
        //Debug.LogWarning("placedGameObjects serverrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr = " + (placedGameObjects.Count - 1));
    }

    internal void RemoveOBjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }
        Destroy(placedGameObjects[gameObjectIndex]);
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
