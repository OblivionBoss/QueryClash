using UnityEngine;
using UnityEngine.EventSystems;

public class ShowInfo : MonoBehaviour
{
    public GameObject InfoPanel;
    public InfoDataManager InfoDataManager;
    void Update()
    {
        // Close the panel if the Escape key is pressed
        if (InfoPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }

        // Close the panel if clicking outside
        if (InfoPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI())
            {
                ClosePanel();
            }
        }
    }

    public void OpenPanel()
    {
        if (InfoPanel != null)
        {
            bool isOpen = InfoPanel.activeSelf;
            InfoPanel.SetActive(!isOpen);
        }
    }

    public void ClosePanel()
    {
        if (InfoPanel != null)
        {
            InfoDataManager.SetCurrentPage(0);
            InfoPanel.SetActive(false);
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
