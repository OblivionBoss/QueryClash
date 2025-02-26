using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using System.Text;

public class QueryStat
{
    public int querySuccess = 0;
    public int queryError = 0;
    public float totalScore = 0f;
    public int[] numOfGetMaterial = new int[6]; // front, sni, shield, cannon, supp, garbage;
}

public class RDBManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject colPrefab;
    public GameObject tablePrefab;
    public Transform canvasParent;
    public Transform queryErrorParent;
    public GameObject linePrefab;
    public Transform linePanel;
    public GameObject queryErrorBoxPrefab;

    [SerializeField] public string[] rdbName;
    private string dbName;
    public InventoryManager inventoryManager;
    public TextMeshProUGUI output;
    public TMP_InputField textInputField;
    [SerializeField] private TMP_Dropdown chooseMatDropdown;
    
    private ResourceDatabase resourceDatabase;
    private GameObject queryResult;
    private List<QueryMaterial> queryMaterialList = new List<QueryMaterial>();
    private MaterialType focusMaterialType;

    private GameObject queryErrorBox;

    public SQLTokenKeyboardManager keyboardManager;
    public TextMeshProUGUI textForSize;

    public Timer timer;
    public SingleTimer singleTimer;
    public QueryStat queryStat = new();

    public bool isNetwork = false;

    // Start is called before the first frame update
    void Start()
    {
        //Random.InitState(randomSeed);
        //var table_name = new List<string>();
        //var all_table_list = new List<List<List<string>>>();
        //getDatabase(dbName, table_name, all_table_list);
        //GenerateAllTables(table_name, all_table_list);

        //dbName = "URI=file:" + Application.streamingAssetsPath + "/RDBs/" + tempDB;
        //Vector2[] company = {new Vector2(380f, 735.88f), new Vector2(-245f, 678f), new Vector2(417.39f, 302f), new Vector2(-430f, 215f), new Vector2(-8f, 99f)};
        //Debug.Log(dbName);

        //resourceDatabase = new ResourceDatabase(dbName, canvasParent, tablePrefab, colPrefab, cellPrefab, keyboardManager, textForSize, company);
        //queryResult = null;
        //GetFocusMaterial();

        //if (!isNetwork)
        //{
        //    if (SingleSceneManager.singleSceneManager.rdb < rdbName.Length)
        //        StartRDB(rdbName[SingleSceneManager.singleSceneManager.rdb]);
        //    else
        //        StartRDB(rdbName[0]);
        //}

        //GameObject a = Instantiate(linePrefab, linePanel);
        //var b = resourceDatabase.GetTable(resourceDatabase.GetTableNames()[0]).getTable();
        //var c = resourceDatabase.GetTable(resourceDatabase.GetTableNames()[1]).getTable();
        //a.transform.GetComponent<LineRendererUi>().CreateLine(b, c, Color.green);
        //GenerateQueryMaterialForAllTableResourceData();

        //database.text = debug_getDatabase(table_name, all_table_list);
        //debug_getResourceTable(resourceTable);
    }

    public void StartRDB(string rdbName)
    {
        dbName = "URI=file:" + Application.streamingAssetsPath + "/RDBs/" + rdbName;
        Vector2[] company = { new Vector2(380f, 735.88f), new Vector2(-245f, 678f), new Vector2(417.39f, 302f), new Vector2(-430f, 215f), new Vector2(-8f, 99f) };
        Debug.Log(dbName);

        resourceDatabase = new ResourceDatabase(dbName, canvasParent, tablePrefab, colPrefab, cellPrefab, keyboardManager, textForSize, company);
        queryResult = null;
        GetFocusMaterial();
    }

    public void GetFocusMaterial()
    {
        focusMaterialType = (MaterialType) chooseMatDropdown.value;
        Debug.Log("focusMaterialType = " + focusMaterialType.ToString());
    }

    public void Query(string query_command)
    {
        if (queryResult != null)
        {
            foreach (Transform line in linePanel)
            {
                Destroy(line.gameObject);
            }
            Destroy(queryResult);
        }
        if (queryErrorBox != null) Destroy(queryErrorBox);

        string temp = "";
        string[] column_select_list = null;
        string modify_query_com = "";
        List<List<string>> query_list = new List<List<string>>(); // idx [i][0] is a column name | idx [i][1] to [i][k] is a data
        List<List<GameObject>> query_cell_list = new List<List<GameObject>>(); // idx [i][0] to [i][k-1] is a query cell in table
        List<List<Sprite>> mat_sprites = new List<List<Sprite>>();
        List<ResourceData> queryData_list = new List<ResourceData>();
        int NumDiffTableQueryFocus = 0, NumMatOtherQueryFocus = 0, QueryFocusCount = 0, QueryNotFocusCount = 0, QueryEmptyCount = 0;

        StringBuilder stringBuilder = new StringBuilder();
        if (isNetwork)
        {
            if (timer.isCountingDown.Value) stringBuilder.Append("#P-");
            else stringBuilder.Append("#B-");
            stringBuilder.Append(timer.timerText.text);
        }
        else
        {
            if (singleTimer.isCountingDown) stringBuilder.Append("#P-");
            else stringBuilder.Append("#B-");
            stringBuilder.Append(singleTimer.timerText.text);
        }

        try
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    try // Compile SQL Query Command
                    {
                        command.CommandText = query_command;
                        IDataReader reader = command.ExecuteReader();
                        reader.Close();
                    }
                    catch (SqliteException e)
                    {
                        temp = e.Message;
                        Debug.LogError(e);
                        queryErrorBox = GenerateQueryErrorBox(e.Message);
                        queryStat.queryError++;
                        connection.Close();

                        // log E with query_command
                        stringBuilder.Append("-E# {");
                        stringBuilder.Append(query_command.Replace("\n", " ").Replace("\r", " "));
                        stringBuilder.Append("}");
                        Debug.LogError(stringBuilder.ToString());
                        return;
                    }

                    // "Modify SQL Command" and "get column name list of first SELECT" for Material query
                    string query_com = query_command;
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
                                        else
                                        {
                                            QueryNotFocusCount++;
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
                        queryErrorBox = GenerateQueryErrorBox("SQL Syntax Error");
                        queryStat.queryError++;

                        Debug.LogError("Modify SQL Command ERROR -> " + e.Message);
                        connection.Close();
                        // log G with query_command
                        stringBuilder.Append("-G# {");
                        stringBuilder.Append(query_command.Replace("\n", " ").Replace("\r", " "));
                        stringBuilder.Append("}");
                        Debug.LogError(stringBuilder.ToString());
                        return;
                    }
                }
                connection.Close();
            }
        }
        catch (Exception e)
        {
            temp = e.Message;
            queryErrorBox = GenerateQueryErrorBox("SQL Syntax Error");
            queryStat.queryError++;

            Debug.LogError("SQLite Connection ERROR -> " + e.Message);
            // log G with query_command
            stringBuilder.Append("-G# {");
            stringBuilder.Append(query_command.Replace("\n", " ").Replace("\r", " "));
            stringBuilder.Append("}");
            Debug.LogError(stringBuilder.ToString());
            return;
        }

        var (log_fst_temp, logMatList) = CalculateQueryMatScore(queryData_list, NumDiffTableQueryFocus, NumMatOtherQueryFocus, QueryFocusCount, QueryNotFocusCount, QueryEmptyCount);
        var (totalScoreAdd, totalScoreNotAdd, numAddMat, numGarbageNotAdd) = AddQueryToInventory();
        if (queryData_list.Count == 0) queryErrorBox = GenerateQueryErrorBox("Empty query result");
        queryStat.querySuccess++;

        string log_fst = log_fst_temp + $",{totalScoreNotAdd.ToString("F2")},{numAddMat}| ";
        // log S
        stringBuilder.Append("-S# {");
        stringBuilder.Append(query_command.Replace("\n", " ").Replace("\r", " "));
        stringBuilder.Append("} ");
        stringBuilder.Append(log_fst);
        stringBuilder.Append(logMatList);
        Debug.LogError(stringBuilder.ToString());

        //numGarbageNotAdd decrease base hp

        //queryResult = GenerateQueryResultTable(query_list, query_cell_list, mat_sprites);
    }

    public (string, string) CalculateQueryMatScore(List<ResourceData> queryData_list, int NumDiffTableQueryFocus, int NumMatOtherQueryFocus, int QueryFocusCount, int QueryNotFocusCount, int QueryEmptyCount)
    {   
        Debug.Log($"{NumDiffTableQueryFocus}, {NumMatOtherQueryFocus}, {QueryFocusCount}, {QueryNotFocusCount}, {QueryEmptyCount}");
        StringBuilder sbForMatList = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        int[] matTypeCount = new int[5];
        int[] matTypeCountAndGarbage = new int[6];
        float totalScore = 0f;

        sbForMatList.Append("[");
        foreach (ResourceData queryCell in queryData_list)
        {
            int duplicateQueryCount = queryCell.GetAndResetDuplicateQueryCount() - 1;
            QueryMaterial material = queryCell.GetMaterial();
            if (material != null)
            {
                material.score = ScoreFunction(NumDiffTableQueryFocus, NumMatOtherQueryFocus, QueryFocusCount, QueryNotFocusCount, QueryEmptyCount, duplicateQueryCount);
                queryMaterialList.Add(material);

                matTypeCount[(int) material.type]++;
                totalScore += material.score;

                matTypeCountAndGarbage[material.score > 0 ? (int) material.type : 5]++;
            }

            // [(<matType>,D,score),...]
            sbForMatList.Append($"({(int) material.type},{duplicateQueryCount},{material.score.ToString("F2")}),");
        }
        sbForMatList.Append("]");

        queryStat.totalScore += totalScore;
        for (int i = 0; i < queryStat.numOfGetMaterial.Length; i++)
        {
            queryStat.numOfGetMaterial[i] += matTypeCountAndGarbage[i];
        }

        // [<focus>,<fro>,<sni>,<shi>,<can>,<sup>]
        sb.Append($"[{(int) focusMaterialType}");
        foreach (int count in matTypeCount)
        {
            sb.Append($",{count}");
        }
        sb.Append("]");

        // |T,O,F,N,E,<totalScore>,<totalScoreNotAdd>,<numAddMat>|
        sb.Append($" |{NumDiffTableQueryFocus},{NumMatOtherQueryFocus},{QueryFocusCount},{QueryNotFocusCount},{QueryEmptyCount},{totalScore.ToString("F2")}");
        return (sb.ToString(), sbForMatList.ToString());
    }

    private float ScoreFunction(float NumDiffTableQueryFocus, float NumMatOtherQueryFocus, float QueryFocusCount, float QueryNotFocusCount, float QueryEmptyCount, float duplicateQueryCount)
    {
        float a = 0.5f, b = 20f, c = 0.1f, d = 100f, e = 100f, f = 50f, g = 50f;
        float T = NumDiffTableQueryFocus; // >= 0
        float O = NumMatOtherQueryFocus; // 0 <= O <= 4
        float F = QueryFocusCount; // >= 0
        float N = QueryNotFocusCount; // >= 0
        float E = QueryEmptyCount; // >= 0
        float D = duplicateQueryCount; // >= 0
        Debug.Log("T = " + T + " F = " + F + " O = " + O + " D = " + D + " E = " + E);
        float score = Mathf.Pow(T, 1.2f) * (b/c) * Mathf.Pow(F, 1.5f) / (F + N + E) - (d * D) - (e * E) - (g * N);
        Debug.Log(score);
        return score;
    }

    public (float, float, int, int) AddQueryToInventory()
    {
        int numAddMat = 0, numGarbageNotAdd = 0;
        float totalScoreNotAdd = 0f, totalScoreAdd = 0f; ;
        foreach (QueryMaterial queryItem in queryMaterialList)
        {
            if (inventoryManager.AddItem(queryItem))
            {
                totalScoreAdd += queryItem.score;
                numAddMat++;
            }
            else
            {
                totalScoreNotAdd += queryItem.score;
                if (queryItem.score <= 0) numGarbageNotAdd++;
            }
        }
        queryMaterialList.Clear();
        return (totalScoreAdd, totalScoreNotAdd, numAddMat, numGarbageNotAdd);
    }

    public static GameObject Instantiate(GameObject tablePrefab, GameObject canvasParent)
    {
        return Instantiate(tablePrefab, canvasParent);
    }

    public GameObject GenerateQueryErrorBox(string errorMessage)
    {
        GameObject queryErrorBox = Instantiate(queryErrorBoxPrefab, queryErrorParent);
        //queryErrorBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(-280f, 500f);
        queryErrorBox.transform.Find("ErrorTextBox").GetComponent<TextMeshProUGUI>().text = errorMessage;
        return queryErrorBox;
    }

    private GameObject GenerateQueryResultTable(List<List<string>> query_result, List<List<GameObject>> cell_list, List<List<Sprite>> mat_sprites)
    {
        // Create a new table
        GameObject newTable = Instantiate(tablePrefab, canvasParent);
        newTable.GetComponent<RectTransform>().anchoredPosition = new Vector2(-280f, 500f);

        Transform tableNameBox = newTable.transform.Find("TableName");
        TextMeshProUGUI tableNameText = tableNameBox.Find("TableNameText").GetComponent<TextMeshProUGUI>();
        tableNameText.fontSize = 25f;
        tableNameText.text = "Query Result";
        var tableNametextSize = FindTextSize(25f, "Query Result");
        float tableNameWidth = tableNametextSize.Item1 * 1.2f;
        float tableNameHeight = tableNametextSize.Item2 * 1.5f;
        RectTransform tableNameRT = tableNameBox.GetComponent<RectTransform>();
        tableNameRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tableNameWidth);
        tableNameRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tableNameHeight);

        Transform tableData = newTable.transform.Find("TableData");

        // Create columns within the table
        var colPair = GenerateTableColumns(tableData, query_result, cell_list, mat_sprites);
        float[] colWidthList = colPair.Item1;
        float columnHeight = colPair.Item2;

        float sumOfColLen = 0f;
        foreach (float len in colWidthList) sumOfColLen += len;
        RectTransform tablePrefabRT = newTable.GetComponent<RectTransform>();
        tablePrefabRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(sumOfColLen, tableNameWidth));
        tablePrefabRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tableNameHeight + columnHeight);

        tableData.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, columnHeight);

        return newTable;
    }

    private (float[], float) GenerateTableColumns(Transform tableData, List<List<string>> query_result,
        List<List<GameObject>> cell_list, List<List<Sprite>> mat_sprites)
    {
        // Find each column width
        float[] colWidthList = new float[query_result.Count];
        float colNameHeight = 20f;
        float dataTextHeight = 15f;
        int numRowToShow = Math.Min(10, query_result[0].Count - 1);
        for (int i = 0; i < query_result.Count; i++)
        {
            var colTextSize = FindTextSize(20f, query_result[i][0]);
            colWidthList[i] = colTextSize.Item1;
            colNameHeight = colTextSize.Item2;
            for (int j = 1; j <= numRowToShow; j++) // query_result[i].Count
            {
                var textSize = FindTextSize(16f, query_result[i][j]);
                colWidthList[i] = Mathf.Max(colWidthList[i], textSize.Item1);
                dataTextHeight = textSize.Item2;
            }
        }

        colNameHeight *= 1.5f;
        float columnDataHeight = 0f;
        if (query_result.Count > 0) columnDataHeight = numRowToShow * (dataTextHeight + 40f);
        float columnHeight = columnDataHeight + colNameHeight;
        for (int i = 0; i < colWidthList.Length; i++) colWidthList[i] *= 1.2f;

        for (int i = 0; i < query_result.Count; i++)
        {
            // Create a new column
            GameObject newColumn = Instantiate(colPrefab, tableData);

            Transform colNameBox = newColumn.transform.Find("ColumnName");
            TextMeshProUGUI colNameText = colNameBox.Find("ColumnNameText").GetComponent<TextMeshProUGUI>();
            colNameText.fontSize = 20f;
            colNameText.text = query_result[i][0];
            colNameBox.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, colNameHeight);

            Transform columnData = newColumn.transform.Find("ColumnData");

            // Create cells within the table
            GenerateColumnCells(columnData, query_result[i], cell_list[i], mat_sprites[i], dataTextHeight, numRowToShow);

            RectTransform colPrefabRT = newColumn.GetComponent<RectTransform>();
            colPrefabRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, colWidthList[i]);
            colPrefabRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, columnHeight);

            columnData.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, columnDataHeight);
        }

        return (colWidthList, columnHeight);
    }

    private void GenerateColumnCells(Transform column, List<string> data_list, List<GameObject> target_cells,
        List<Sprite> mat_sprites, float dataTextHeight, int numRowToShow)
    {
        for (int i = 1; i <= numRowToShow; i++) // data_list.Count
        {
            // Create a new cell
            GameObject newCell = Instantiate(cellPrefab, column);

            RectTransform cellRT = newCell.GetComponent<RectTransform>();
            cellRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dataTextHeight + 40f);
            Transform cellDataSlot = newCell.transform.Find("DataTextSlot");
            cellDataSlot.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dataTextHeight);

            // Set the icon (Image component)
            Image iconImage = newCell.transform.Find("ItemIcon").Find("MaterialIcon").GetComponent<Image>();
            iconImage.sprite = mat_sprites[i-1];

            // Set the text (Text component)
            TextMeshProUGUI cellDataText = cellDataSlot.Find("DataText").GetComponent<TextMeshProUGUI>();
            cellDataText.fontSize = 16f;
            cellDataText.text = data_list[i];

            GameObject line = Instantiate(linePrefab, linePanel);
            line.transform.GetComponent<LineRendererUi>().CreateLine(target_cells[i-1], newCell, Color.magenta);
        }
    }

    private (float, float) FindTextSize(float fontsize, string text)
    {
        textForSize.fontSize = fontsize;
        textForSize.text = text;
        textForSize.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textForSize.preferredWidth);
        textForSize.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textForSize.preferredHeight);
        Debug.Log(textForSize.text + " fontsize = " + textForSize.fontSize + " width " + textForSize.preferredWidth + " height " + textForSize.preferredHeight);
        return (textForSize.preferredWidth, textForSize.preferredHeight);
    }

    //public void ExecuteForInputText()
    //{
    //    try { Query(textInputField.text); }
    //    catch { }
    //    try { Query1(textInputField.text); }
    //    catch { }
    //}

    //public void Query1(string query_command)
    //{
    //    string temp = "";

    //    using (var connection = new SqliteConnection(dbName))
    //    {
    //        connection.Open();
    //        try
    //        {
    //            using (var command = connection.CreateCommand())
    //            {
    //                command.CommandText = query_command;

    //                using (IDataReader reader = command.ExecuteReader())
    //                {
    //                    DataTable schema = reader.GetSchemaTable();
    //                    temp += "| ";
    //                    foreach (DataRow col in schema.Rows)
    //                    {
    //                        temp += col.Field<string>("ColumnName") + " | ";
    //                    }
    //                    temp += "\n";

    //                    while (reader.Read())
    //                    {
    //                        temp += "| ";
    //                        for (int i = 0; i < reader.FieldCount; i++)
    //                        {
    //                            temp += reader[i] + " | ";
    //                        }
    //                        temp += "\n";
    //                    }
    //                    reader.Close();
    //                }
    //            }
    //        }
    //        catch (SqliteException e)
    //        {
    //            temp = e.Message;
    //            Debug.Log(e);
    //            output.text = temp;
    //            throw e;
    //        }
    //        connection.Close();
    //    }
    //    output.text = temp;
    //}

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

    //private string debug_getDatabase(List<string> table_name, List<List<List<string>>> all_table_list)
    //{
    //    string text = string.Empty;
    //    for (int i = 0; i < all_table_list.Count; i++)
    //    {
    //        text += table_name[i] + "\n";
    //        for (int j = 0; j < all_table_list[i][0].Count; j++) // loop each column in table[i]
    //        {
    //            for (int k = 0; k < all_table_list[i].Count; k++)
    //            {
    //                text += all_table_list[i][k][j] + " | ";
    //            }
    //            text += "\n";
    //        }
    //        text += "\n\n";
    //    }
    //    return text;
    //}

    //public void getDatabase(string dbName, List<string> table_name, List<List<List<string>>> all_table_list)
    //{
    //    using (var connection = new SqliteConnection(dbName))
    //    {
    //        connection.Open();
    //        try
    //        {
    //            using (var command = connection.CreateCommand())
    //            {
    //                command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name != 'sqlite_sequence'";

    //                using (IDataReader reader = command.ExecuteReader())
    //                {
    //                    while (reader.Read())
    //                    {
    //                        Debug.Log(reader[0].ToString());
    //                        table_name.Add(reader[0].ToString());
    //                    }
    //                    reader.Close();
    //                }

    //                foreach (var table in table_name)
    //                {
    //                    var table_list = new List<List<string>>();
    //                    command.CommandText = "SELECT * FROM " + table;

    //                    using (IDataReader reader = command.ExecuteReader())
    //                    {
    //                        DataTable schema = reader.GetSchemaTable();
    //                        foreach (DataRow col in schema.Rows)
    //                        {
    //                            var col_list = new List<string>();
    //                            col_list.Add(col.Field<string>("ColumnName"));
    //                            table_list.Add(col_list);
    //                        }

    //                        while (reader.Read())
    //                        {
    //                            IDataRecord data = (IDataRecord)reader;
    //                            for (int i = 0; i < data.FieldCount; i++)
    //                            {
    //                                table_list[i].Add(reader[i].ToString());
    //                            }
    //                        }
    //                        reader.Close();
    //                    }
    //                    all_table_list.Add(table_list);
    //                }
    //            }
    //        }
    //        catch (SqliteException e)
    //        {
    //            Debug.Log(e);
    //            throw(e);
    //        }
    //        connection.Close();
    //    }
    //}
}