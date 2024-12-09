using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnButton : MonoBehaviour
{
    public GameObject itemPrefab; // Assign the prefab this button should spawn
    private InventoryManager inventoryManager;

    private void Start()
    {
        // Find the InventoryManager in the scene
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    /// <summary>
    /// Spawns the assigned item prefab into the inventory.
    /// </summary>
    //public void SpawnItem()
    //{
    //    if (inventoryManager != null && itemPrefab != null)
    //    {
    //        inventoryManager.AddItemToUnitInventory(itemPrefab);
    //    }
    //    else
    //    {
    //        Debug.LogError("InventoryManager or itemPrefab is not assigned!");
    //    }
    //}
}
