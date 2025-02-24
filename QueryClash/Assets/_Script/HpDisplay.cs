using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpDisplay : MonoBehaviour
{
    public TextMeshProUGUI leftHpText;
    public TextMeshProUGUI rightHpText;

    public RectTransform leftHpRT;
    public RectTransform rightHpRT;

    public Image leftHpBar;
    public Image rightHpBar;

    public GameObject LeftBluebase;
    public GameObject LeftRedbase;

    public GameObject RightRedbase;
    public GameObject RightBluebase;

    public GameObject Wall;

    public void swapHpUi()
    {
        Vector3 leftHpBarPos = leftHpRT.transform.localPosition;
        leftHpRT.transform.localPosition = rightHpRT.transform.localPosition;
        rightHpRT.transform.localPosition = leftHpBarPos;

        Color leftHpBarColor = leftHpBar.color;
        leftHpBar.color = rightHpBar.color;
        rightHpBar.color = leftHpBarColor;

        leftHpBar.fillOrigin = 1;
        rightHpBar.fillOrigin = 0;
        
        Color leftHpColor = leftHpText.color;
        leftHpText.color = rightHpText.color;
        rightHpText.color = leftHpColor;

        LeftBluebase.SetActive(false);
        LeftRedbase.SetActive(true);

        RightRedbase.SetActive(false);
        RightBluebase.SetActive(true);

        Wall.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        Wall.transform.position = new Vector3(0f, 0f, 0.8f);
    }
}