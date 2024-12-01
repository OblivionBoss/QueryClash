using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Transform inventorySlotsParent; // Parent object containing all inventory slots

    /// <summary>
    /// Spawns a new item in the inventory at the first available slot.
    /// </summary>
    /// <param name="itemPrefab">The prefab to instantiate.</param>
    public void AddItemToInventory(GameObject itemPrefab)
    {
        // Find the first empty slot
        foreach (Transform slot in inventorySlotsParent)
        {
            if (slot.childCount == 0)
            {
                // Instantiate the item and set it as a child of the slot
                GameObject newItem = Instantiate(itemPrefab, slot);

                // Optionally customize the new item (e.g., sprite, name)
                return;
            }
        }

        Debug.LogWarning("No empty slots available in the inventory!");
    }
}
