using System;
using System.IO;
using UnityEngine;

public class QueryLogger : MonoBehaviour
{
    private string logFilePath;

    private void Start()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "query_log.txt");
        Debug.LogError(logFilePath);
    }

    public void LogQuery(string query)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timestamp} - {query}";

        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        Debug.Log($"Logged: {logEntry}");
    }
}