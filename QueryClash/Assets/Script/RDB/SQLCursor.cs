using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SQLCursor : MonoBehaviour
{
    public RectTransform rectTransform;
    public Image cursor;
    private Color cursorHide;
    private Color cursorShow;

    private void Start()
    {
        cursorShow = cursor.color;
        cursorShow.a = 1;
        cursorHide = cursor.color;
        cursorHide.a = 0;
        StartCoroutine(SwitchColor());
    }

    private IEnumerator SwitchColor()
    {
        bool toggle = false;
        while (true)
        {
            cursor.color = toggle ? cursorHide : cursorShow;
            toggle = !toggle;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
