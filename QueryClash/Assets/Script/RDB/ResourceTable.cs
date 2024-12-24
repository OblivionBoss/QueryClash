using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;
using System;
using TMPro;

public class ResourceTable
{
    private string tableName;
    private int numOfCol = 0;
    private string[] columnNameList;
    private string[] columnDatatypeList;
    private Dictionary<string, List<ResourceData>> columnList;
    private string primaryKey = null;
    private List<Tuple<string, string, string>> foreignKeyList; // {this.colName, fk.tableName, fk.colName}
    private GameObject resourceTable;
    // database.get("tableName").get("columnName")[i]

    public string[] getColumnNameList()
    {
        return columnNameList;
    }

    public GameObject getTable()
    {
        return resourceTable;
    }

    public string getPrimaryKey()
    {
        return primaryKey;
    }

    public string getTableName()
    {
        return tableName;
    }

    public ResourceData GetData(string column, string pk_id, string dataKey)
    {
        List<ResourceData> pk_column_datas;
        columnList.TryGetValue(primaryKey, out pk_column_datas);
        int data_idx = pk_column_datas.FindIndex(x => x.GetData().Equals(pk_id));
        if (data_idx < 0)
        {
            Debug.LogError($"pk_id {pk_id} does not exists in {tableName}");
            throw new System.Exception();
        }

        if (column.Equals(primaryKey))
        {
            return pk_column_datas[data_idx];
        }
        else
        {
            List<ResourceData> column_datas;
            columnList.TryGetValue(column, out column_datas);
            ResourceData data = column_datas[data_idx];
            if (data.GetData().Equals(dataKey)) { return data; }
            else
            {
                Debug.LogError($"{dataKey} not found in {tableName}");
                throw new System.Exception();
            }
        }
    }

    public ResourceTable(string dbName, string tablename, Transform canvasParent, GameObject tablePrefab,
        GameObject colPrefab, GameObject cellPrefab, SQLTokenKeyboardManager keyboardManager) // call constructor by ResourceDatabase Class
    {
        tableName = tablename; // set table name
        columnList = new Dictionary<string, List<ResourceData>>();
        foreignKeyList = new List<Tuple<string, string, string>>();

        resourceTable = RDBManager.Instantiate(tablePrefab, canvasParent);
        resourceTable.name = tableName;

        Transform tableNameBox = resourceTable.transform.Find("TableName");
        TextMeshProUGUI tableNameText = tableNameBox.Find("TableNameText").GetComponent<TextMeshProUGUI>();
        tableNameText.text = tableName;
        tableNameBox.GetComponent<TableTokenButton>().Setup(keyboardManager, tablename);

        // You can optionally reposition the table or set its RectTransform
        //RectTransform tableRectTransform = resourceTable.GetComponent<RectTransform>();
        //tableRectTransform.anchoredPosition = new Vector2(i * 70, 0); // Stack tables vertically

        Transform tableData = resourceTable.transform.Find("TableData");

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            try
            {
                using (var command = connection.CreateCommand())
                {
                    // GET primary key info AND column names OF this table FROM sqlite database
                    command.CommandText = $"PRAGMA table_info({tablename})";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        List<string> col_name_list = new List<string>();
                        List<string> col_datatype_list = new List<string>();
                        while (reader.Read())
                        {
                            string col_name = reader[1].ToString();
                            col_name_list.Add(col_name);

                            string col_datatype = reader[2].ToString();
                            col_datatype_list.Add(col_datatype);
                            Debug.Log($"{col_name} has type = {reader[2].ToString()}");

                            if (reader[5].ToString().Equals("1"))
                            {
                                if (primaryKey == null)
                                {
                                    primaryKey = col_name;
                                    Debug.Log($"{col_name} is a pk. At cid = {numOfCol}");
                                }
                                else
                                {
                                    Debug.LogError($"this \"{tableName}\" table has more than 1 pk. {{}}");
                                }
                            }
                            numOfCol++;
                        }
                        reader.Close();
                        columnNameList = col_name_list.ToArray();
                        columnDatatypeList = col_datatype_list.ToArray();
                    }

                    // GET foreign key info OF this table FROM sqlite database
                    command.CommandText = $"PRAGMA foreign_key_list({tablename})";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string from_col = reader[3].ToString();
                            string to_table = reader[2].ToString();
                            string to_col = reader[4].ToString();
                            foreignKeyList.Add(new Tuple<string, string, string>(from_col, to_table, to_col));
                            Debug.Log($"this \"{tableName}.{from_col}\" has fk to \"{to_table}.{to_col}\"");
                        }
                        reader.Close();
                    }

                    // GET all data in this table FROM sqlite database
                    List<ResourceData>[] table_list = new List<ResourceData>[numOfCol];
                    GameObject[] col_gameobject_list = new GameObject[numOfCol];
                    for (int i = 0; i < numOfCol; i++)
                    {
                        table_list[i] = new List<ResourceData>();

                        col_gameobject_list[i] = RDBManager.Instantiate(colPrefab, tableData);
                        col_gameobject_list[i].name = columnNameList[i];

                        Transform colNameBox = col_gameobject_list[i].transform.Find("ColumnName");
                        TextMeshProUGUI colNameText = colNameBox.Find("ColumnNameText").GetComponent<TextMeshProUGUI>();
                        colNameText.text = columnNameList[i];
                        colNameBox.GetComponent<ColumnTokenButton>().Setup(keyboardManager, columnNameList[i]);
                    }
                    command.CommandText = "SELECT * FROM " + tablename;
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Transform columnData = col_gameobject_list[i].transform.Find("ColumnData");
                                ResourceData data = new ResourceData(reader[i].ToString(), cellPrefab, columnData, columnDatatypeList[i], keyboardManager);
                                table_list[i].Add(data);
                            }
                        }
                        reader.Close();
                    }
                    for (int i = 0; i < numOfCol; i++)
                    {
                        columnList.Add(columnNameList[i], table_list[i]);
                    }
                }
            }
            catch (SqliteException e)
            {
                Debug.LogError(e);
                throw (e);
            }
            connection.Close();
        }
    }

}
