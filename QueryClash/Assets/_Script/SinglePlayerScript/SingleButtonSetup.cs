using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SingleButtonSetup : MonoBehaviour//, IPointerClickHandler
{
    public Button button;
    public int prefabIndex;
    public float score;

    public Image unitInventorySlot;

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

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
        if (unitInventorySlot != null) unitInventorySlot.color = Color.white;
    }
}
