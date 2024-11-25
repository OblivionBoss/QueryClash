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

    public ResourceData(string data, GameObject cellPrefab, Transform columnTransform)
    {
        this.data = data;
        cell = RDBManager.Instantiate(cellPrefab, columnTransform);
        cell.name = data;

        //Transform iconImage = cell.transform.Find("ItemIcon");
        Image iconImage = cell.transform.Find("ItemIcon").GetComponent<Image>();
        materialGenerator = GameObject.FindAnyObjectByType<QueryMaterialManager>();
        material = materialGenerator.GenerateQueryMaterial(iconImage);

        // Set the text (Text component)
        TextMeshProUGUI cellDataText = cell.transform.Find("DataText").GetComponent<TextMeshProUGUI>();
        cellDataText.text = data;

        //Node newNode = new Node(newCell, type, data_list[i]);
        //cellsInColumn.Add(newNode);
        //cell.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);
    }

    public QueryMaterial GetMaterial() {
        cell.GetComponent<CellBehaviour>().call();
        return material;
    }

    public string getData()
    {
        return data;
    }

}
