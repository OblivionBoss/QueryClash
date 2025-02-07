using TMPro;
using UnityEngine;
using UnityEngine.UI; // Use TMPro if you're using TextMeshPro

public class SingleHpDisplay : MonoBehaviour
{
    public TMP_Text leftHpText; // Use TMP_Text for TextMeshPro
    public TMP_Text rightHpText;

    public GameObject leftBase; // Reference to the left Base object
    public GameObject rightBase; // Reference to the right Base object

    private SingleBase leftBaseScript;
    private SingleBase rightBaseScript;

    void Start()
    {
        // Get the Base script from the game objects
        leftBaseScript = leftBase.GetComponent<SingleBase>();
        rightBaseScript = rightBase.GetComponent<SingleBase>();
    }

    void Update()
    {
        // Update the text with the current HP values
        leftHpText.text = leftBaseScript.CurrentHp.ToString("#0");
        rightHpText.text = rightBaseScript.CurrentHp.ToString("#0");
    }
}
