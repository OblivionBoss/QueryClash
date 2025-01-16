using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CellBehaviour : MonoBehaviour//, IPointerClickHandler
{
    [SerializeField] private Transform cellMaterial;
    [SerializeField] private GameObject cellMaterialSlot;
    [SerializeField] private GameObject cellCooldownText;
    private bool isCooldown = false;
    private float timeLeft = 30;
    private Image icon;
    private TextMeshProUGUI cdText;

    // Start is called before the first frame update
    void Start()
    {
        icon = cellMaterialSlot.GetComponent<Image>();
        cdText = cellCooldownText.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCooldown)
        {
            timeLeft -= Time.deltaTime;
            cdText.text = timeLeft.ToString("#0");
            if (timeLeft < 0) NotCooldown();
        }
    }

    public bool IsCooldown()
    {
        return isCooldown;
    }

    public bool call()
    {
        if (!isCooldown)
        {
            cellCooldownText.SetActive(true);
            cellMaterialSlot.SetActive(false);

            icon.enabled = false;
            cdText.enabled = true;
            isCooldown = true;
            return true;
        }
        return false;
    }

    private void NotCooldown()
    {
        cellCooldownText.SetActive(false);
        cellMaterialSlot.SetActive(true);

        icon.enabled = true;
        cdText.enabled = false;
        isCooldown = false;
        timeLeft = 30f;
    }
}
