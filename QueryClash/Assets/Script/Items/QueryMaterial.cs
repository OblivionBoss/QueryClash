using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Inventory/Material")]
public class QueryMaterial : Item
{
    public float score = 0;
    public MaterialType type;
    private Quality quality = Quality.None;

    public override void Use()
    {
        base.Use();
        // Move to Crafting slot
        // Remove it from Inventory
    }

    public Color GetQuality()
    {
        if (quality == Quality.None)
        {
            if (score > 800f) quality = Quality.Legendary;
            else if (score > 600f) quality = Quality.Epic;
            else if (score > 400f) quality = Quality.Rare;
            else if (score > 200f) quality = Quality.Common;
            else quality = Quality.Gabbage;
        }
        Debug.Log(quality.ToString() + $" score = {score}");
        switch (quality)
        {
            case Quality.Legendary:
                return new Color(1f, 0.743384f, 0f);
            case Quality.Epic:
                return new Color(0.7693181f, 0f, 1f);
            case Quality.Rare:
                return new Color(0f, 0.5488603f, 1f);
            case Quality.Common:
                return new Color(0f, 0.7987421f, 0.07062242f);
            case Quality.Gabbage:
                return new Color(0.6289307f, 0.6289307f, 0.6289307f);
            default:
                return new Color(1f, 1f, 1f);
        }
    }
}

public enum MaterialType
{
    Frontline,
    Sniper,
    Shielder,
    Cannon,
    Support
}

public enum Quality
{
    Legendary,
    Epic,
    Rare,
    Common,
    Gabbage,
    None
}