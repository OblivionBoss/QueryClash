using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CellBehaviour : MonoBehaviour//, IPointerClickHandler
{
    [SerializeField] private Transform cellMaterial;
    [SerializeField] private Transform cellCooldownText;
    private bool isCooldown = false;
    private float timeLeft = 10;
    private Image icon;
    private TextMeshProUGUI cdText;

    // Start is called before the first frame update
    void Start()
    {
        icon = cellMaterial.GetComponent<Image>();
        cdText = cellCooldownText.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCooldown)
        {
            timeLeft -= Time.deltaTime;
            cdText.text = timeLeft.ToString("#.00");
            if (timeLeft < 0 ) NotCooldown();
        }
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    icon.enabled = false;
    //    cdText.enabled = true;
    //    isCooldown = true;
    //}

    public bool call()
    {
        if (!isCooldown)
        {
            icon.enabled = false;
            cdText.enabled = true;
            isCooldown = true;
            return true;
        }
        return false;
    }

    private void NotCooldown()
    {
        icon.enabled = true;
        cdText.enabled = false;
        isCooldown = false;
        timeLeft = 10;
    }
}
