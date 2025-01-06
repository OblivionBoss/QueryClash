using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IDropHandler
{
    public CraftingManager craftingManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if (inventoryItem.item is QueryMaterial)
            {
                inventoryItem.parentAfterDrag = transform;
                inventoryItem.CraftingSlot = this;
            }
        }
    }
}