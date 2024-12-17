using UnityEngine;
using UnityEngine.EventSystems;

public class SQLInline : MonoBehaviour, IPointerClickHandler
{
    public SQLLine thisline;

    public void OnPointerClick(PointerEventData eventData)
    {
        thisline.keyboardManager.OnClickLine(thisline);
    }
}
