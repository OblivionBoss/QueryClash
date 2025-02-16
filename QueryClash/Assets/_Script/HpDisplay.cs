using TMPro;
using UnityEngine;
using UnityEngine.UI; // Use TMPro if you're using TextMeshPro

public class HpDisplay : MonoBehaviour
{
    public TMP_Text leftHpText; // Use TMP_Text for TextMeshPro
    public TMP_Text rightHpText;

    public GameObject leftBase; // Reference to the left Base object
    public GameObject rightBase; // Reference to the right Base object

    private Base leftBaseScript;
    private Base rightBaseScript;

    public RectTransform leftHpRT;
    public RectTransform rightHpRT;

    public Image leftHpBar;
    public Image rightHpBar;

    void Start()
    {
        // Get the Base script from the game objects
        leftBaseScript = leftBase.GetComponent<Base>();
        rightBaseScript = rightBase.GetComponent<Base>();
    }

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
        
        //Vector3 leftHpPos = leftHpText.transform.localPosition;
        //leftHpText.transform.localPosition = rightHpText.transform.localPosition;
        //rightHpText.transform.localPosition = leftHpPos;
    }

    //void Update()
    //{
    //    // Update the text with the current HP values
    //    leftHpText.text = leftBaseScript.CurrentHp.Value.ToString("#0");
    //    rightHpText.text = rightBaseScript.CurrentHp.Value.ToString("#0");
    //}
}
