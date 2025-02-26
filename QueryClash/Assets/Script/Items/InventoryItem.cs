using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;

    [HideInInspector] public Item item;
    [HideInInspector] public CraftingSlot CraftingSlot;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public bool isDestroy = false;

    public void InitializeItem(Item item)
    {
        this.item = item;
        image.sprite = item.icon;
        if (item is QueryMaterial)
        {
            var material = (QueryMaterial) item;
            image.color = material.GetQuality();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
        if(CraftingSlot != null)
        {
            CraftingSlot.craftingManager.UpdateCrafting();
        }
        if (isDestroy)
        {
            if (item is QueryMaterial)
            {
                var material = (QueryMaterial) item;
                if (material.score <= 0)
                {
                    RDBManager rdbm = GameObject.Find("RDBManager").GetComponent<RDBManager>();
                    if (rdbm != null) rdbm.ReduceBaseHpWhenDeleteGarbage(1);
                }
            }
            Destroy(gameObject);
        }
    }
}