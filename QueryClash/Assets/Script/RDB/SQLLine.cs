using System.Collections;
using UnityEngine;

public class SQLLine : MonoBehaviour
{
    public RectTransform rectTransform;
    public RectTransform line;
    public RectTransform lineHead;
    public SQLLineHead SQLLineHead;
    public SQLInline SQLInline;
    public SQLTokenKeyboardManager keyboardManager;

    public int lineNumber; // generate by SQLToken...Manager
    public int lineHeadToken;
    public int lastTokenOfLine;

    //public void AddKeyboardManager(SQLTokenKeyboardManager keyboardManager)
    //{
    //    if (this.keyboardManager == null)
    //    {
    //        this.keyboardManager = keyboardManager;
    //    }
    //}
}
