using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellTokenButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public SQLTokenKeyboardManager keyboardManager;
    public string data;
    public string datatype;

    private Vector3 beginClickPosition;

    public void Setup(SQLTokenKeyboardManager keyboardManager, string data, string datatype)
    {
        this.keyboardManager = keyboardManager;
        this.data = data;
        this.datatype = datatype;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (beginClickPosition == transform.position)
        {
            if (datatype.Equals("INTEGER") || datatype.Equals("REAL"))
                keyboardManager.AddNumberToken(data);
            else
                keyboardManager.AddStringToken(data);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        beginClickPosition = transform.position;
    }
}
