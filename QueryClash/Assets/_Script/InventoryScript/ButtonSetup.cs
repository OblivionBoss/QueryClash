using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSetup : MonoBehaviour, IPointerClickHandler
{
    public Button button;
    public int prefabIndex;
    public float score;

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        // Find the PlacementSystem in the scene
        PlacementSystem placementSystem = FindObjectOfType<PlacementSystem>();

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
}
