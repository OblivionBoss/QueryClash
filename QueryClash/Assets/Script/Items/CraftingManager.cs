using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public CraftingSlot[] CraftingSlots;
    public Image resultSlot;
    public Image unitCraftingIcon;

    public Transform[] UnitInventorySlots;
    public GameObject UnitIconPrefab;
    public GameObject[] UnitIconPrefabList;

    //public UnitCraftingIcon[] UnitCraftingIconList;
    public Sprite[] UnitSpriteList;
    private List<int[]> CraftingRecipe;

    private int unitIndex;
    private float score;
    private List<InventoryItem> craftingMaterials;

    public void UpdateCrafting()
    {
        QueryMaterial[] pattern = new QueryMaterial[CraftingSlots.Length];
        craftingMaterials = new List<InventoryItem>();
        for (int i = 0; i < CraftingSlots.Length; i++)
        {
            CraftingSlot slot = CraftingSlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                pattern[i] = null;
                //Debug.Log("Slot " + i + " = null");
            }
            else
            {
                QueryMaterial materialHold = (QueryMaterial) itemInSlot.item;
                pattern[i] = materialHold;
                craftingMaterials.Add(itemInSlot);
                //Debug.Log("Slot " + i + " = " + materialHold.type + " " + (int) materialHold.type);
            }
        }
        int[] patt = ChangePatternFormat(pattern);
        CheckCraftPattern(patt, pattern);
    }

    public void OnClickCraft()
    {
        if (AddUnitIcon())
        {
            RemoveUnitCraftingIcon();
            ClearCraftingMaterials();
        }
        else
        {
            Debug.Log("Unit Inventory full");
        }
    }

    private void ClearCraftingMaterials()
    {
        for (int i = 0; i < CraftingSlots.Length; i++)
        {
            CraftingSlot slot = CraftingSlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null) Destroy(itemInSlot.gameObject);
        }
    }

    private void CheckCraftPattern(int[] pattern, QueryMaterial[] materials)
    {
        for (int i = 0; i < CraftingRecipe.Count; i++)
        {
            if (CheckPattern(CraftingRecipe[i], pattern))
            {
                Debug.Log("craft!!!!!!!!!!");
                Crafting(materials, i);
                AddUnitCraftingIcon(i);
                return;
            }
        }
        Debug.Log("Nothing to craft!!!!!!!!!!");
        RemoveUnitCraftingIcon();
    }

    private void Crafting(QueryMaterial[] materials, int id)
    {
        float total = 0;
        int amount = 0;
        foreach (QueryMaterial material in materials)
        {
            if(material != null)
            {
                total += material.score;
                amount++;
            }
        }
        unitIndex = id;
        score = total / amount;
    }

    private void AddUnitCraftingIcon(int unit_id)
    {
        resultSlot.color = GetQuality(score);
        unitCraftingIcon.enabled = true;
        unitCraftingIcon.sprite = UnitSpriteList[unit_id];
    }

    private void RemoveUnitCraftingIcon()
    {
        resultSlot.color = Color.white;
        unitCraftingIcon.enabled = false;
        unitCraftingIcon.sprite = null;
        unitIndex = -1;
        score = 0;
    }

    private bool CheckPattern(int[] recipe, int[] pattern)
    {
        for (int i = 0; i < 9; i++)
        {
            if (recipe[i] != pattern[i]) return false;
        }
        return true;
    }

    private int[] ChangePatternFormat(QueryMaterial[] pattern)
    {
        int[] newPattern = new int[pattern.Length];
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == null) newPattern[i] = -1;
            else newPattern[i] = (int) pattern[i].type;
        }
        return newPattern;
    }

    private void SetRecipe()
    {
        CraftingRecipe = new List<int[]>();
        int[] burst = { -1, 0, -1, -1, 0, -1, -1, 0, -1 };
        CraftingRecipe.Add(burst);
        int[] sustain = { -1, -1, -1, 0, 4, 0, -1, -1, -1 };
        CraftingRecipe.Add(sustain);
        int[] sniper1 = { -1, 1, -1, -1, 1, -1, -1, 1, -1 };
        CraftingRecipe.Add(sniper1);
        int[] sniper2 = { -1, -1, -1, 1, 1, 1, -1, -1, -1 };
        CraftingRecipe.Add(sniper2);
        int[] cannon = { 3, -1, 3, -1, 3, -1, 3, -1, 3 };
        CraftingRecipe.Add(cannon);
        int[] laser = { 3, -1, 3, -1, 1, -1, 3, -1, 3 };
        CraftingRecipe.Add(laser);
        int[] defensive = { 2, -1, 2, -1, 2, -1, -1, 2, -1 };
        CraftingRecipe.Add(defensive);
        int[] counterAtk = { -1, 2, -1, -1, 0, -1, 2, -1, 2 };
        CraftingRecipe.Add(counterAtk);
        int[] healer = { -1, 4, -1, 4, -1, 4, -1, 4, -1 };
        CraftingRecipe.Add(healer);
        int[] buffer = { 4, -1, 4, -1, -1, -1, 4, -1, 4 };
        CraftingRecipe.Add(buffer);
    }

    void Start()
    {
        SetRecipe();
        ClearCraftingMaterials();
        RemoveUnitCraftingIcon();
    }

    public bool AddUnitIcon()
    {
        for (int i = 0; i < UnitInventorySlots.Length; i++)
        {
            Transform slot = UnitInventorySlots[i];
            if (slot.childCount == 0)
            {
                SpawnNewUnitIcon(slot);
                return true;
            }
        }
        return false;
    }

    void SpawnNewUnitIcon(Transform slot)
    {
        GameObject newUnitIcon = Instantiate(UnitIconPrefabList[unitIndex], slot);
        ButtonSetup unit = newUnitIcon.GetComponent<ButtonSetup>();
        //unit.prefabIndex = unitIndex;
        unit.score = score;
        unit.unitInventorySlot = slot.GetComponent<Image>();
        //newUnitIcon.GetComponent<Image>().sprite = UnitSpriteList[unitIndex];
        unit.unitInventorySlot.color = GetQuality(score);
    }

    private Color GetQuality(float score)
    {
        if (score > 1200f) return new Color(1f, 0.743384f, 0f);
        else if (score > 800f) return new Color(0.7693181f, 0f, 1f);
        else if (score > 400f) return new Color(0f, 0.5488603f, 1f);
        else return new Color(0f, 0.7987421f, 0.07062242f);
    }
}