using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceData
{
    private string data;
    private QueryMaterial material;
    private GameObject cell;
    private QueryMaterialManager materialGenerator;
    private Image materialImage;

    public ResourceData(string data, GameObject cellPrefab, Transform columnTransform)
    {
        this.data = data;
        cell = RDBManager.Instantiate(cellPrefab, columnTransform);
        cell.name = data;

        //Transform iconImage = cell.transform.Find("ItemIcon");
        materialImage = cell.transform.Find("ItemIcon").GetComponent<Image>();
        materialGenerator = GameObject.FindAnyObjectByType<QueryMaterialManager>();
        material = materialGenerator.GenerateQueryMaterial(materialImage);

        // Set the text (Text component)
        TextMeshProUGUI cellDataText = cell.transform.Find("DataText").GetComponent<TextMeshProUGUI>();
        cellDataText.text = data;

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

    public QueryMaterial GetMaterial(out Sprite icon)
    {
        if (cell.GetComponent<CellBehaviour>().call())
        {
            QueryMaterial gotMaterial = material;
            icon = materialImage.sprite;
            material = materialGenerator.GenerateQueryMaterial(materialImage);
            return gotMaterial;
        }
        else
        {
            icon = null;
            return null;
        }
    }

    public GameObject getCell()
    {
        return cell;
    }

    public string getData()
    {
        return data;
    }

}
