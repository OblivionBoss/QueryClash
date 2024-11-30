using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterUnit", menuName = "Inventory/CharacterUnit")]
public class CharacterUnit : Item
{
    public double HP;
    public double ATK;
    public double DEF;
    // more..

    public override void Use()
    {
        base.Use();
        // use it to spawn unit on the field
        // Remove it from Inventory
    }
}
