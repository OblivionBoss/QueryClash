using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Timers;
using static UnityEditor.Progress;

public enum MaterialType
{
    Type1,
    Type2,
    Type3
}

public class Node
{
    public GameObject cell;
    public MaterialType type;
    public string data;
    public float cooldown = 0;

    public Node(GameObject cell, int type, string data)
    {
        this.cell = cell;
        this.type = (MaterialType)type;
        this.data = data;
    }

    public MaterialType getMaterial()
    {
        Image iconImage = cell.transform.Find("ItemIcon").GetComponent<Image>();
        iconImage.sprite = null;
        cooldown = 3;
        return type;
    }
}

public class RDBManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject colPrefab;
    public GameObject tablePrefab;
    public Transform canvasParent;

    public TextMeshProUGUI database;
    public Sprite[] icon;
    public string dbName = "URI=file:RDB.db";
    public int randomSeed = 42;

    public Dictionary<string, List<Node>> resourceTable = new Dictionary<string, List<Node>>();

    public TextMeshProUGUI output;
    public TMP_InputField query_command;

    // Start is called before the first frame update
    void Start()
    {
        Random.seed = randomSeed;
        var table_name = new List<string>();
        var all_table_list = new List<List<List<string>>>();
        getDatabase(dbName, table_name, all_table_list);
        GenerateAllTables(table_name, all_table_list);
        //database.text = debug_getDatabase(table_name, all_table_list);
        debug_getResourceTable(resourceTable);
    }

    public void getResource()
    {
        var query_list = new List<List<string>>();
        try
        {
            Query(query_list);
            foreach (var col in query_list) {
                List<Node> column;
                string key = col[0];
                if (resourceTable.TryGetValue(key, out column))
                {
                    foreach (Node node in column)
                    {
                        for (int i = 1; i< col.Count; i++)
                        {
                            if (node.data.Equals(col[i]))
                            {
                                node.getMaterial();
                                col.Remove(col[i]);
                                break;
                            }
                        }
                    }
                    // maybe col.Count > 1
                }
            }
        }
        catch
        {

        }
    }

    public void Query(List<List<string>> query_list)
    {
        string temp = "";

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query_command.text;

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        DataTable schema = reader.GetSchemaTable();
                        temp += "| ";
                        foreach (DataRow col in schema.Rows)
                        {
                            var col_list = new List<string>();
                            col_list.Add(col.Field<string>("ColumnName"));
                            query_list.Add(col_list);

                            temp += col.Field<string>("ColumnName") + " | ";
                        }
                        temp += "\n";

                        while (reader.Read())
                        {
                            IDataRecord data = (IDataRecord)reader;
                            temp += "| ";
                            for (int i = 0; i < data.FieldCount; i++)
                            {
                                query_list[i].Add(reader[i].ToString());

                                temp += reader[i] + " | ";
                            }
                            temp += "\n";
                        }
                        reader.Close();
                    }
                }
            }
            catch (SqliteException e)
            {
                temp = e.Message;
                Debug.Log(e);
                throw e;
            }
            connection.Close();
        }
        output.text = temp;
    }

    public void GenerateAllTables(List<string> table_name, List<List<List<string>>> all_table_list)
    {
        for (int i = 0; i < table_name.Count; i++)
        {
            // Create a new table
            GameObject newTable = Instantiate(tablePrefab, canvasParent);

            TextMeshProUGUI tableNameText = newTable.transform.Find("TableName").Find("TableNameText").GetComponent<TextMeshProUGUI>();
            tableNameText.text = table_name[i];

            // You can optionally reposition the table or set its RectTransform
            RectTransform tableRectTransform = newTable.GetComponent<RectTransform>();
            tableRectTransform.anchoredPosition = new Vector2(i * 70, 0); // Stack tables vertically

            Transform tableData = newTable.transform.Find("TableData");

            // Create columns within the table
            GenerateTableColumns(tableData, all_table_list[i]);
        }
    }

    public void GenerateTableColumns(Transform tableData, List<List<string>> col_list)
    {
        for (int i = 0; i < col_list.Count; i++)
        {
            // Create a new column
            GameObject newColumn = Instantiate(colPrefab, tableData);

            TextMeshProUGUI colNameText = newColumn.transform.Find("ColumnName").Find("ColumnNameText").GetComponent<TextMeshProUGUI>();
            colNameText.text = col_list[i][0];

            Transform columnData = newColumn.transform.Find("ColumnData");

            List<Node> cellsInColumn = new List<Node>();
            // Create cells within the table
            GenerateColumnCells(columnData, col_list[i], cellsInColumn);

            try
            {
                resourceTable.Add(col_list[i][0], cellsInColumn);
            }
            catch
            {
                resourceTable.Add(col_list[i][0] + i, cellsInColumn);
            }
            
        }
    }

    public void GenerateColumnCells(Transform column, List<string> data_list, List<Node> cellsInColumn)
    {
        for (int i = 1; i < data_list.Count; i++)
        {
            // Create a new cell
            GameObject newCell = Instantiate(cellPrefab, column);

            int type = Random.Range(0, icon.Length);

            // Set the icon (Image component)
            Image iconImage = newCell.transform.Find("ItemIcon").GetComponent<Image>();
            iconImage.sprite = icon[type];

            // Set the text (Text component)
            TextMeshProUGUI cellDataText = newCell.transform.Find("DataText").GetComponent<TextMeshProUGUI>();
            cellDataText.text = data_list[i];

            Node newNode = new Node(newCell, type, data_list[i]);
            cellsInColumn.Add(newNode);
        }
    }

    private void debug_getResourceTable(Dictionary<string, List<Node>> resourceTable)
    {
        foreach (var item in resourceTable)
        {
            List<Node> temp;
            string key = item.Key;
            string p = key;
            if (resourceTable.TryGetValue(key, out temp))
            {
                foreach (var cell in temp)
                {
                    var c = cell.cell;
                    TextMeshProUGUI cellDataText = c.transform.Find("DataText").GetComponent<TextMeshProUGUI>();
                    p += cellDataText.text + " ";
                }
            }
            Debug.Log(p);
        }
    }

    private string debug_getDatabase(List<string> table_name, List<List<List<string>>> all_table_list)
    {
        string text = string.Empty;
        for (int i = 0; i < all_table_list.Count; i++)
        {
            text += table_name[i] + "\n";
            for (int j = 0; j < all_table_list[i][0].Count; j++) // loop each column in table[i]
            {
                for (int k = 0; k < all_table_list[i].Count; k++)
                {
                    text += all_table_list[i][k][j] + " | ";
                }
                text += "\n";
            }
            text += "\n\n";
        }
        return text;
    }

    public void getDatabase(string dbName, List<string> table_name, List<List<List<string>>> all_table_list)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name != 'sqlite_sequence'";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.Log(reader[0].ToString());
                            table_name.Add(reader[0].ToString());
                        }
                        reader.Close();
                    }

                    foreach (var table in table_name)
                    {
                        var table_list = new List<List<string>>();
                        command.CommandText = "SELECT * FROM " + table;

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            DataTable schema = reader.GetSchemaTable();
                            foreach (DataRow col in schema.Rows)
                            {
                                var col_list = new List<string>();
                                col_list.Add(col.Field<string>("ColumnName"));
                                table_list.Add(col_list);
                            }

                            while (reader.Read())
                            {
                                IDataRecord data = (IDataRecord)reader;
                                for (int i = 0; i < data.FieldCount; i++)
                                {
                                    table_list[i].Add(reader[i].ToString());
                                }
                            }
                            reader.Close();
                        }
                        all_table_list.Add(table_list);
                    }
                }
            }
            catch (SqliteException e)
            {
                Debug.Log(e);
                throw(e);
            }
            connection.Close();
        }
    }
}
