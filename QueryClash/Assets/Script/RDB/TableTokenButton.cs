using UnityEngine;
using UnityEngine.EventSystems;

public class TableTokenButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public SQLTokenKeyboardManager keyboardManager;
    public string tablename;

    private Vector3 beginClickPosition;

    public void Setup(SQLTokenKeyboardManager keyboardManager, string tablename)
    {
        this.keyboardManager = keyboardManager;
        this.tablename = tablename;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (beginClickPosition == transform.position)
            keyboardManager.AddTableNameToken(tablename);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        beginClickPosition = transform.position;
    }
}