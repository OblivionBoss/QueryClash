using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SQLLineHead : MonoBehaviour, IPointerClickHandler
{
    private SQLTokenKeyboardManager keyboardManager;
    public SQLLine thisline;

    public void OnPointerClick(PointerEventData eventData)
    {
        keyboardManager.OnClickLineHead(thisline);
    }

    public void AddKeyboardManager(SQLTokenKeyboardManager keyboardManager)
    {
        if (this.keyboardManager == null)
        {
            this.keyboardManager = keyboardManager;
        }

    }
}
