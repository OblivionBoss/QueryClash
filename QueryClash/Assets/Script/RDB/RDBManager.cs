using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class RDBManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject colPrefab;
    public GameObject tablePrefab;
    public Transform canvasParent;
    public GameObject linePrefab;
    public Transform linePanel;

    public string dbName = "URI=file:shop.db";
    public InventoryManager inventoryManager;
    public TextMeshProUGUI output;
    public TMP_InputField query_command;
    [SerializeField] private TMP_Dropdown chooseMatDropdown;
    
    private ResourceDatabase resourceDatabase;
    private GameObject queryResult;
    private List<QueryMaterial> queryMaterialList = new List<QueryMaterial>();
    private MaterialType focusMaterialType;

    // Start is called before the first frame update
    void Start()
    {
        //Random.InitState(randomSeed);
        //var table_name = new List<string>();
        //var all_table_list = new List<List<List<string>>>();
        //getDatabase(dbName, table_name, all_table_list);
        //GenerateAllTables(table_name, all_table_list);

        resourceDatabase = new ResourceDatabase(dbName, canvasParent, tablePrefab, colPrefab, cellPrefab);
        queryResult = null;
        GetFocusMaterial();
        //GameObject a = Instantiate(linePrefab, linePanel);
        //var b = resourceDatabase.GetTable(resourceDatabase.GetTableNames()[0]).getTable();
        //var c = resourceDatabase.GetTable(resourceDatabase.GetTableNames()[1]).getTable();
        //a.transform.GetComponent<LineRendererUi>().CreateLine(b, c, Color.green);
        //GenerateQueryMaterialForAllTableResourceData();

        //database.text = debug_getDatabase(table_name, all_table_list);
        //debug_getResourceTable(resourceTable);
    }

    //public QueryMaterial GenerateQueryMaterial(Transform materialSlot)
    //{
    //    Array values = Enum.GetValues(typeof(QueryMaterialType));
    //    int rand = UnityEngine.Random.Range(0, values.Length);
    //    return new QueryMaterial(Instantiate(queryMaterials[rand], materialSlot), (QueryMaterialType)values.GetValue(rand));
    //}

    //public void GenerateQueryMaterialForAllTableResourceData()
    //{
    //    string[] tables = resourceDatabase.GetTableNames();
    //    foreach (string table in tables)
    //    {
    //        resourceDatabase.GetTable(table);
    //    }
    //}

    public void GetFocusMaterial()
    {
        focusMaterialType = (MaterialType) chooseMatDropdown.value;
        Debug.Log("focusMaterialType = " + focusMaterialType.ToString());
    }

    public void Query()
    {
        if (queryResult != null)
        {
            foreach (Transform line in linePanel)
            {
                Destroy(line.gameObject);
            }
            Destroy(queryResult);
        }
        string temp = "";
        string[] column_select_list = null;
        string modify_query_com = "";
        List<List<string>> query_list = new List<List<string>>(); // idx [i][0] is a column name | idx [i][1] to [i][k] is a data
        List<List<GameObject>> query_cell_list = new List<List<GameObject>>(); // idx [i][0] to [i][k-1] is a query cell in table
        List<List<Sprite>> mat_sprites = new List<List<Sprite>>();
        List<ResourceData> queryData_list = new List<ResourceData>();
        int NumDiffTableQueryFocus = 0, NumMatOtherQueryFocus = 0, QueryFocusCount = 0, QueryEmptyCount = 0;
        try
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    try // "Modify SQL Command" and "get column name list of first SELECT" for Material query
                    {
                        command.CommandText = query_command.text;
                        IDataReader reader = command.ExecuteReader();
                        reader.Close();

                        string query_com = query_command.text;
                        query_com = Regex.Replace(query_com, @"\n|\r", " ");
                        string[] parts = query_com.Split('(');
                        Regex regex = new Regex("(?i)select(?-i)(.*)(?i)from(?-i)");
                        bool first = true;
                        foreach (string s in parts)
                        {
                            Match col_section_match = regex.Match(s);
                            if (col_section_match.Success)
                            {
                                string replace_com = Regex.Replace(s, @"(?i)select(?-i)(.*)(?i)from(?-i)", "SELECT * FROM");
                                Debug.Log(s + " -> " + col_section_match.Success + " => " + col_section_match.ToString() + " >> " + replace_com);
                                if (first)
                                {
                                    string first_sel = col_section_match.ToString();
                                    first_sel = Regex.Replace(first_sel, @"\s+", "");
                                    string col_name_list = first_sel.Remove(first_sel.Length - 4).Substring(6);
                                    Debug.Log(col_name_list);
                                    column_select_list = col_name_list.Split(',');
                                    modify_query_com += replace_com;
                                    first = false;
                                }
                                else
                                {
                                    modify_query_com += '(' + replace_com;
                                }
                            }
                            else
                            {
                                modify_query_com += '(' + s;
                                Debug.Log(s + " -> " + col_section_match.Success);
                            }
                        }
                    }
                    catch (SqliteException e)
                    {
                        temp = e.Message;
                        Debug.LogError(e);
                        throw e;
                    }

                    try // using Modify SQL Command to Query
                    {
                        List<List<string>> query_all_list = new List<List<string>>();
                        Debug.Log(modify_query_com);
                        command.CommandText = modify_query_com;
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            DataTable schema = reader.GetSchemaTable();
                            temp += "| ";
                            foreach (DataRow col in schema.Rows)
                            {
                                var col_list = new List<string>();
                                col_list.Add(col.Field<string>("ColumnName"));
                                query_all_list.Add(col_list);

                                temp += col.Field<string>("ColumnName") + " | ";
                            }
                            temp += "\n";

                            while (reader.Read())
                            {
                                temp += "| ";
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    query_all_list[i].Add(reader[i].ToString());
                                    temp += reader[i] + " | ";
                                }
                                temp += "\n";
                            }
                            reader.Close();
                        }

                        HashSet<string> DiffTableQueryFocusSet = new HashSet<string>();
                        HashSet<MaterialType> MatOtherQueryFocusSet = new HashSet<MaterialType>();
                        foreach (string column_select in column_select_list)
                        {
                            ResourceTable column_select_table = resourceDatabase.GetResourceTableFromColumnName(column_select);
                            string pk = column_select_table.getPrimaryKey();
                            int column_select_idx = -1, pk_idx = -1;
                            for (int i = 0; i < query_all_list.Count; i++) // find index of "column_select" and "pk of that col" in "query_all_list"
                            {
                                if (column_select.Equals(query_all_list[i][0]))
                                {
                                    column_select_idx = i;
                                }
                                if (pk.Equals(query_all_list[i][0]))
                                {
                                    pk_idx = i;
                                }
                            }

                            if (column_select_idx == -1)
                            {
                                Debug.LogError($"this select column \"{column_select}\" not exists in query_all_result");
                            }
                            if (pk_idx == -1)
                            {
                                Debug.LogError($"this pk \"{pk}\" not exists in query_all_result");
                            }
                            query_list.Add(query_all_list[column_select_idx]);
                            List<GameObject> qc_list = new List<GameObject>();
                            List<Sprite> sprite_list = new List<Sprite>();
                            bool isFound = false;
                            for (int i = 1; i < query_all_list[column_select_idx].Count; i++)
                            {
                                ResourceData queryMaterialCell = column_select_table.GetData(column_select, query_all_list[pk_idx][i], query_all_list[column_select_idx][i]);
                                qc_list.Add(queryMaterialCell.GetCell());
                                Sprite gotMatIcon = null;
                                //QueryMaterial material = queryMaterialCell.GetMaterial(out gotMatIcon);
                                //if (material != null)
                                //{
                                //    queryMaterialList.Add(material);
                                //}
                                if (!queryMaterialCell.IsCooldown())
                                {
                                    if (queryMaterialCell.Query(out gotMatIcon))
                                    {
                                        queryData_list.Add(queryMaterialCell);
                                        MaterialType matType = queryMaterialCell.GetQueryMaterial().type;
                                        MatOtherQueryFocusSet.Add(matType);
                                        if (matType.Equals(focusMaterialType))
                                        {
                                            QueryFocusCount++;
                                            if (!isFound)
                                            {
                                                DiffTableQueryFocusSet.Add(column_select_table.getTableName());
                                                isFound = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    QueryEmptyCount++;
                                }
                                sprite_list.Add(gotMatIcon);
                            }
                            query_cell_list.Add(qc_list);
                            mat_sprites.Add(sprite_list);
                        }
                        NumDiffTableQueryFocus = DiffTableQueryFocusSet.Count;
                        NumMatOtherQueryFocus = MatOtherQueryFocusSet.Count;
                        if (MatOtherQueryFocusSet.Contains(focusMaterialType)) NumMatOtherQueryFocus--;
                    }
                    catch (SqliteException e)
                    {
                        temp = e.Message;
                        Debug.LogError("Modify SQL Command ERROR -> " + e.Message);
                        throw e;
                    }
                }
                connection.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SQLite Connection ERROR -> " + e.Message);
            throw e;
        }
        queryResult = GenerateQueryResultTable(query_list, query_cell_list, mat_sprites);
        CalculateQueryMatScore(queryData_list, NumDiffTableQueryFocus, NumMatOtherQueryFocus, QueryFocusCount, QueryEmptyCount);
        AddQueryToInventory();
    }

    public void CalculateQueryMatScore(List<ResourceData> queryData_list, int NumDiffTableQueryFocus, int NumMatOtherQueryFocus, int QueryFocusCount, int QueryEmptyCount)
    {
        Debug.Log($"{NumDiffTableQueryFocus}, {NumMatOtherQueryFocus}, {QueryFocusCount}, {QueryEmptyCount}");
        foreach (ResourceData queryCell in queryData_list)
        {
            int duplicateQueryCount = queryCell.GetAndResetDuplicateQueryCount() - 1;
            QueryMaterial material = queryCell.GetMaterial();
            if (material != null)
            {
                material.score = ScoreFunction(NumDiffTableQueryFocus, NumMatOtherQueryFocus, QueryFocusCount, QueryEmptyCount, duplicateQueryCount);
                queryMaterialList.Add(material);
            }
        }
    }

    private float ScoreFunction(float NumDiffTableQueryFocus, float NumMatOtherQueryFocus, float QueryFocusCount, float QueryEmptyCount, float duplicateQueryCount)
    {
        float a = 100f, b = 500f, c = 0.5f, d = 0.5f, e = 0.5f;
        float T = NumDiffTableQueryFocus;
        float O = NumMatOtherQueryFocus;
        float F = QueryFocusCount;
        float E = QueryEmptyCount;
        float D = duplicateQueryCount;
        float score = (a * T + b * F * F) / ((c * O * O + 1) * (d * D + 1) * (e * E + 1));
        Debug.Log(score);
        return score;
    }

    public void AddQueryToInventory()
    {
        foreach (QueryMaterial queryItem in queryMaterialList)
        {
            inventoryManager.AddItem(queryItem);
        }
        queryMaterialList.Clear();
    }

    public void Query1()
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
                            temp += col.Field<string>("ColumnName") + " | ";
                        }
                        temp += "\n";

                        while (reader.Read())
                        {
                            temp += "| ";
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
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
                output.text = temp;
                throw e;
            }
            connection.Close();
        }
        output.text = temp;
    }

    public static GameObject Instantiate(GameObject tablePrefab, GameObject canvasParent)
    {
        return Instantiate(tablePrefab, canvasParent);
    }

    public GameObject GenerateQueryResultTable(List<List<string>> query_result, List<List<GameObject>> cell_list, List<List<Sprite>> mat_sprites)
    {
        // Create a new table
        GameObject newTable = Instantiate(tablePrefab, canvasParent);

        TextMeshProUGUI tableNameText = newTable.transform.Find("TableName").Find("TableNameText").GetComponent<TextMeshProUGUI>();
        tableNameText.text = "Query Result";

        Transform tableData = newTable.transform.Find("TableData");

        // Create columns within the table
        GenerateTableColumns(tableData, query_result, cell_list, mat_sprites);

        return newTable;
    }

    public void GenerateTableColumns(Transform tableData, List<List<string>> query_result, List<List<GameObject>> cell_list, List<List<Sprite>> mat_sprites)
    {
        for (int i = 0; i < query_result.Count; i++)
        {
            // Create a new column
            GameObject newColumn = Instantiate(colPrefab, tableData);

            TextMeshProUGUI colNameText = newColumn.transform.Find("ColumnName").Find("ColumnNameText").GetComponent<TextMeshProUGUI>();
            colNameText.text = query_result[i][0];

            Transform columnData = newColumn.transform.Find("ColumnData");

            // Create cells within the table
            GenerateColumnCells(columnData, query_result[i], cell_list[i], mat_sprites[i]);
        }
    }

    public void GenerateColumnCells(Transform column, List<string> data_list, List<GameObject> target_cells, List<Sprite> mat_sprites)
    {
        for (int i = 1; i < data_list.Count; i++)
        {
            // Create a new cell
            GameObject newCell = Instantiate(cellPrefab, column);

            // Set the icon (Image component)
            Image iconImage = newCell.transform.Find("ItemIcon").GetComponent<Image>();
            iconImage.sprite = mat_sprites[i-1];

            // Set the text (Text component)
            TextMeshProUGUI cellDataText = newCell.transform.Find("DataText").GetComponent<TextMeshProUGUI>();
            cellDataText.text = data_list[i];

            GameObject line = Instantiate(linePrefab, linePanel);
            line.transform.GetComponent<LineRendererUi>().CreateLine(target_cells[i-1], newCell, Color.magenta);
        }
    }

    //private void debug_getResourceTable(Dictionary<string, List<Node>> resourceTable)
    //{
    //    foreach (var item in resourceTable)
    //    {
    //        List<Node> temp;
    //        string key = item.Key;
    //        string p = key;
    //        if (resourceTable.TryGetValue(key, out temp))
    //        {
    //            foreach (var cell in temp)
    //            {
    //                var c = cell.cell;
    //                TextMeshProUGUI cellDataText = c.transform.Find("DataText").GetComponent<TextMeshProUGUI>();
    //                p += cellDataText.text + " ";
    //            }
    //        }
    //        Debug.Log(p);
    //    }
    //}

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

    //public void GenerateAllTables(List<string> table_name, List<List<List<string>>> all_table_list)
    //{
    //    for (int i = 0; i < table_name.Count; i++)
    //    {
    //        // Create a new table
    //        GameObject newTable = Instantiate(tablePrefab, canvasParent);

    //        TextMeshProUGUI tableNameText = newTable.transform.Find("TableName").Find("TableNameText").GetComponent<TextMeshProUGUI>();
    //        tableNameText.text = table_name[i];

    //        // You can optionally reposition the table or set its RectTransform
    //        RectTransform tableRectTransform = newTable.GetComponent<RectTransform>();
    //        tableRectTransform.anchoredPosition = new Vector2(i * 70, 0); // Stack tables vertically

    //        Transform tableData = newTable.transform.Find("TableData");

    //        // Create columns within the table
    //        GenerateTableColumns(tableData, all_table_list[i]);
    //    }
    //}

    //public void GenerateTableColumns(Transform tableData, List<List<string>> col_list)
    //{
    //    for (int i = 0; i < col_list.Count; i++)
    //    {
    //        // Create a new column
    //        GameObject newColumn = Instantiate(colPrefab, tableData);

    //        TextMeshProUGUI colNameText = newColumn.transform.Find("ColumnName").Find("ColumnNameText").GetComponent<TextMeshProUGUI>();
    //        colNameText.text = col_list[i][0];

    //        Transform columnData = newColumn.transform.Find("ColumnData");

    //        List<Node> cellsInColumn = new List<Node>();
    //        // Create cells within the table
    //        GenerateColumnCells(columnData, col_list[i], cellsInColumn);

    //        try
    //        {
    //            resourceTable.Add(col_list[i][0], cellsInColumn);
    //        }
    //        catch
    //        {
    //            resourceTable.Add(col_list[i][0] + i, cellsInColumn);
    //        }

    //    }
    //}

    //public void GenerateColumnCells(Transform column, List<string> data_list, List<Node> cellsInColumn)
    //{
    //    for (int i = 1; i < data_list.Count; i++)
    //    {
    //        // Create a new cell
    //        GameObject newCell = Instantiate(cellPrefab, column);

    //        int type = UnityEngine.Random.Range(0, icon.Length);

    //        // Set the icon (Image component)
    //        Image iconImage = newCell.transform.Find("ItemIcon").GetComponent<Image>();
    //        iconImage.sprite = icon[type];

    //        // Set the text (Text component)
    //        TextMeshProUGUI cellDataText = newCell.transform.Find("DataText").GetComponent<TextMeshProUGUI>();
    //        cellDataText.text = data_list[i];

    //        Node newNode = new Node(newCell, type, data_list[i]);
    //        cellsInColumn.Add(newNode);
    //    }
    //}
}