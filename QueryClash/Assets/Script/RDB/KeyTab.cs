using UnityEngine;

public class KeyTab : MonoBehaviour
{
    public RectTransform panel;
    private Vector3 closedposition;
    private bool isOpen = false;

    private void Start()
    {
        closedposition = panel.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            MoveObject();
        }
    }

    public void MoveObject()
    {
        if (!isOpen)
        {
            panel.position = new Vector3(0, 0);
            isOpen = true;
        }
        else
        {
            panel.position = new Vector3(-Screen.width, 0);
            isOpen = false;
        }
    }
}
