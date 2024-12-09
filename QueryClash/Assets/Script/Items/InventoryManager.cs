using UnityEngine;
using System;
using static UnityEngine.Rendering.DebugUI;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    public Item[] itemAddDemos;

    void Start()
    {
        UnityEngine.Random.InitState(42);
    }

    public void randomAddItem()
    {
        int rand = UnityEngine.Random.Range(0, itemAddDemos.Length);
        this.AddItem(itemAddDemos[rand]);
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return;
            }
        }
    }

    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }


}
