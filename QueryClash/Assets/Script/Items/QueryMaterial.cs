using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Inventory/Material")]
public class QueryMaterial : Item
{
    public double stat;

    public override void Use()
    {
        base.Use();
        // Move to Crafting slot
        // Remove it from Inventory
    }
}
