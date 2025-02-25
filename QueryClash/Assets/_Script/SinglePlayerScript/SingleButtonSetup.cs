using UnityEngine;
using UnityEngine.UI;

public class SingleButtonSetup : MonoBehaviour
{
    public Button button;
    public int prefabIndex;
    public float score;

    public Image unitInventorySlot;
    public Sprite slotSprite;

    void Start()
    {
        // Find the PlacementSystem in the scene
        SinglePlacementSystem placementSystem = FindObjectOfType<SinglePlacementSystem>();

        if (placementSystem != null && button != null)
        {
            // Clear existing listeners
            //button.onClick.RemoveAllListeners();

            // Add a listener and pass the int parameter (6)
            button.onClick.AddListener(() => placementSystem.StartPlacement(prefabIndex, gameObject, score));
        }
        else
        {
            Debug.LogError("PlacementSystem or Button is not set!");
        }
    }

    void OnDestroy()
    {
        Debug.Log("unit button " + prefabIndex + " has been destroy");
        if (unitInventorySlot != null)
        {
            unitInventorySlot.color = Color.white;
            unitInventorySlot.sprite = slotSprite;
        }
    }
}