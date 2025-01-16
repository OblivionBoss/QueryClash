using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceData
{
    private string data;
    private string datatype;
    private QueryMaterial material;
    private GameObject cell;
    private QueryMaterialManager materialGenerator;
    private Image materialImage;

    private int duplicateQueryCount = 0;

    public ResourceData(string data, GameObject cellPrefab, Transform columnTransform, string datatype, SQLTokenKeyboardManager keyboardManager, float width, float textHeight)
    {
        this.data = data;
        this.datatype = datatype;
        cell = RDBManager.Instantiate(cellPrefab, columnTransform);
        cell.name = data;
        RectTransform cellRT = cell.GetComponent<RectTransform>();
        //cellRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        cellRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight + 40f);
        Transform cellDataSlot = cell.transform.Find("DataTextSlot");
        cellDataSlot.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);

        //Transform iconImage = cell.transform.Find("ItemIcon");
        materialImage = cell.transform.Find("ItemIcon").Find("MaterialIcon").GetComponent<Image>();
        materialGenerator = GameObject.FindAnyObjectByType<QueryMaterialManager>();
        material = materialGenerator.GenerateQueryMaterial(materialImage);

        // Set the text (Text component)
        Transform cellData = cellDataSlot.Find("DataText");
        TextMeshProUGUI cellDataText = cellData.GetComponent<TextMeshProUGUI>();
        cellDataText.fontSize = 16f;
        cellDataText.text = data;
        cellData.GetComponent<CellTokenButton>().Setup(keyboardManager, data, datatype);

        //Node newNode = new Node(newCell, type, data_list[i]);
        //cellsInColumn.Add(newNode);
        //cell.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);
    }

    //public QueryMaterial GetMaterial() {
    //    if (cell.GetComponent<CellBehaviour>().call())
    //    {
    //        material = materialGenerator.GenerateQueryMaterial(materialImage);
    //        return material;
    //    }
    //    return null;
    //}

    public QueryMaterial GetMaterial(/*out Sprite icon*/)
    {
        if (cell.GetComponent<CellBehaviour>().call())
        {
            QueryMaterial gotMaterial = material;
            //icon = materialImage.sprite;
            material = materialGenerator.GenerateQueryMaterial(materialImage);
            return gotMaterial;
        }
        else
        {
            //icon = null;
            return null;
        }
    }

    public bool IsCooldown()
    {
        return cell.GetComponent<CellBehaviour>().IsCooldown();
    }

    public bool Query(out Sprite icon)
    {
        bool isFisrtQuery = duplicateQueryCount == 0;
        duplicateQueryCount++;
        if (isFisrtQuery) icon = materialImage.sprite;
        else icon = null;
        return isFisrtQuery;
    }

    public int GetAndResetDuplicateQueryCount()
    {
        int count = duplicateQueryCount;
        duplicateQueryCount = 0;
        return count;
    }

    public QueryMaterial GetQueryMaterial()
    {
        return material;
    }

    public GameObject GetCell()
    {
        return cell;
    }

    public string GetData()
    {
        return data;
    }

}
