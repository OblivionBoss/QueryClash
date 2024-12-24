using UnityEngine;
using UnityEngine.EventSystems;

public class ColumnTokenButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public SQLTokenKeyboardManager keyboardManager;
    public string columnName;

    private Vector3 beginClickPosition;

    public void Setup(SQLTokenKeyboardManager keyboardManager, string columnName)
    {
        this.keyboardManager = keyboardManager;
        this.columnName = columnName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (beginClickPosition == transform.position)
            keyboardManager.AddColumnNameToken(columnName);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        beginClickPosition = transform.position;
    }
}