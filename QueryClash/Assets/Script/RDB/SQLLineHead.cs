using UnityEngine;
using UnityEngine.EventSystems;

public class SQLLineHead : MonoBehaviour, IPointerClickHandler
{
    public SQLLine thisline;

    public void OnPointerClick(PointerEventData eventData)
    {
        thisline.keyboardManager.OnClickLineHead(thisline);
    }
}
