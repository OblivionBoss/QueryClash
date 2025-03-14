using UnityEngine;
using UnityEngine.UI;

public class LineRendererUi : MonoBehaviour
{
    [SerializeField] private RectTransform m_myTransform;
    [SerializeField] private Image m_image;
    private GameObject tableCell = null;
    private GameObject queryCell = null;

    void Update()
    {
        if (tableCell != null && queryCell != null)
        {
            UpdateLine(tableCell.transform.position, queryCell.transform.position);
        }
    }

    public void UpdateLine(Vector3 positionOne, Vector3 positionTwo)
    {
        Vector2 point1 = new Vector2(positionTwo.x, positionTwo.y);
        Vector2 point2 = new Vector2(positionOne.x, positionOne.y);
        Vector2 midpoint = (point1 + point2) / 2f;

        m_myTransform.position = midpoint;

        Vector2 dir = point1 - point2;
        m_myTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        m_myTransform.localScale = new Vector3(dir.magnitude, 1f, 1f);
    }

    public void CreateLine(GameObject tablecell, GameObject querycell, Color color)
    {
        tableCell = tablecell;
        queryCell = querycell;
        m_image.color = color;
        m_image.raycastTarget = false;
    }
}
