using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;
using Mono.Data.Sqlite;

public class ResourceDatabase
{
    private string[] tableNames;
    private Dictionary<string, ResourceTable> resourceTables;
    private Dictionary<string, ResourceTable> mapColumnToTable;

    public string[] GetTableNames()
    {
        return tableNames;
    }

    public ResourceTable GetResourceTableFromColumnName(string columnname)
    {
        ResourceTable resourceTable = null;
        mapColumnToTable.TryGetValue(columnname, out resourceTable);
        return resourceTable;
    }

    public Dictionary<string, ResourceTable> GetResourceTables()
    {
        return resourceTables;
    }

    public ResourceTable GetTable(string tablename)
    {
        ResourceTable table = null;
        resourceTables.TryGetValue(tablename, out table);
        return table;
    }

    public ResourceDatabase(string dbName, Transform canvasParent, GameObject tablePrefab,
        GameObject colPrefab, GameObject cellPrefab, SQLTokenKeyboardManager keyboardManager, TextMeshProUGUI textForSize)
    {
        resourceTables = new Dictionary<string, ResourceTable>();
        mapColumnToTable = new Dictionary<string, ResourceTable>();
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            try
            {
                using (var command = connection.CreateCommand())
                {
                    // GET all table name OF this database FROM sqlite database
                    List<string> table_name = new List<string>();
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name != 'sqlite_sequence'";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.Log(reader[0].ToString());
                            table_name.Add(reader[0].ToString());
                        }
                        reader.Close();
                        tableNames = table_name.ToArray();
                    }

                    foreach (string table in tableNames)
                    {
                        resourceTables.Add(table, new ResourceTable(dbName, table, canvasParent, tablePrefab, colPrefab, cellPrefab, keyboardManager, textForSize));
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
        foreach(var resourceTable in resourceTables.Values)
        {
            string[] col_list = resourceTable.getColumnNameList();
            foreach (string col in col_list)
            {
                mapColumnToTable.Add(col, resourceTable);
            }
        }
    }
}
