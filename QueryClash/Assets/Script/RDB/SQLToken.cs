using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SQLToken : MonoBehaviour, IPointerClickHandler
{
    private SQLTokenKeyboardManager keyboardManager;
    public HorizontalLayoutGroup horizontalForRightPad;
    public TextMeshProUGUI tokenText;
    public RectTransform rectTransform;
    public int tokenIndex;
    public bool isString;

    public void OnPointerClick(PointerEventData eventData)
    {
        keyboardManager.OnClickToken(this);
    }

    public void AddKeyboardManager(SQLTokenKeyboardManager keyboardManager)
    {
        if (this.keyboardManager == null)
        {
            this.keyboardManager = keyboardManager;
        }  
    }
}