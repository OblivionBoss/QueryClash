using UnityEngine;
using UnityEngine.UI;

public class ButtonSetupR : MonoBehaviour
{
    public Button button;
    public int prefabIndex;
    public int grade;
    void Start()
    {
        // Find the PlacementSystem in the scene
        PlacementSystemR placementSystemR = FindObjectOfType<PlacementSystemR>();

        if (placementSystemR!= null && button != null)
        {
            // Clear existing listeners
            //button.onClick.RemoveAllListeners();

            // Add a listener and pass the int parameter (6)
            button.onClick.AddListener(() => placementSystemR.StartPlacement(prefabIndex, gameObject, grade));
        }
        else
        {
            Debug.LogError("PlacementSystem or Button is not set!");
        }
    }
}
