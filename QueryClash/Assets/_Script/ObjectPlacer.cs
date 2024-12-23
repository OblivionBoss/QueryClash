using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position,int grade)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        placedGameObjects.Add(newObject);

        // Check if the placed object has a CapsuleUnit script
        Unit unit = newObject.GetComponent<Unit>();
        if (unit != null)
        {
            // Call the OnPlaced method of the CapsuleUnit
            unit.OnPlaced();
            unit.SetGrade(grade);
        }

        return placedGameObjects.Count - 1;
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
}
