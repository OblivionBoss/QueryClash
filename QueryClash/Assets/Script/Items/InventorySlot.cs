using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
            if (inventoryItem.CraftingSlot != null)
            {
                inventoryItem.CraftingSlot.craftingManager.UpdateCrafting();
            }
            inventoryItem.CraftingSlot = null;
        }
    }
}