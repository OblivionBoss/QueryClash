using System;
using System.IO;
using UnityEngine;

public class QueryLogger : MonoBehaviour
{
    private string timestamp;
    public RDBManager RDBManager;
    public SingleEnemySpawner Spawner;
    public bool isLog = false;

    void Start()
    {
        DateTime utcNow = DateTime.UtcNow;
        TimeZoneInfo thailandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        DateTime thailandTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, thailandTimeZone);
        timestamp = thailandTime.ToString("dd-MM-yyyy_HH-mm-ss-fff");
    }

    public void LogQuery(string queryLog)
    {
        if (!isLog) return;
        if (RDBManager.isNetwork)
        {
            string multiFilePath = Application.streamingAssetsPath + "/QueryLogs/Multiplayer/" + "multi_" + timestamp + ".txt";
            if (!File.Exists(multiFilePath))
            {
                File.WriteAllText(multiFilePath, $"multiplayer {timestamp} {RDBManager.RDBName}\n");
            }
            File.AppendAllText(multiFilePath, queryLog + "\n");
        }
        else
        {
            string singleFilePath = Application.streamingAssetsPath + "/QueryLogs/Singleplayer/" + "single_" + timestamp + ".txt";
            if (!File.Exists(singleFilePath))
            {
                File.WriteAllText(singleFilePath, $"singleplayer {timestamp} {RDBManager.RDBName} {Spawner.difficulty}\n");
            }
            File.AppendAllText(singleFilePath, queryLog + "\n");
        }
        Debug.LogError($"Logged: {queryLog}");
    }
}