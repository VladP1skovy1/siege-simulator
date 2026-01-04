using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Globalization;

public class DataManager
{
    private readonly string _dataFolderPath;

    public DataManager()
    {
        _dataFolderPath = Path.Combine(Application.dataPath, "Data");

        if (!Directory.Exists(_dataFolderPath))
        {
            Directory.CreateDirectory(_dataFolderPath);
            Debug.Log($"[DataManager] Created directory: {_dataFolderPath}");
        }
    }
    
    public void SaveStatisticsToCsv(List<List<float>> statistics, string algorithmName)
    {
       
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{algorithmName}_{timestamp}.csv";
        
        string fullPath = Path.Combine(_dataFolderPath, fileName);
        
        StringBuilder sb = new StringBuilder();
        
        if (statistics.Count > 0 && statistics[0].Count > 0)
        {
            sb.Append("RunID");
            for (int i = 0; i < statistics[0].Count; i++)
            {
                sb.Append($",Epoch_{i}");
            }
            sb.AppendLine();
        }

      
        for (int runIndex = 0; runIndex < statistics.Count; runIndex++)
        {
            List<float> runData = statistics[runIndex];
            sb.Append($"Run_{runIndex}");

            foreach (float fitness in runData)
            {
                sb.Append("," + fitness.ToString(CultureInfo.InvariantCulture));
            }
            sb.AppendLine();
        }
        
        try 
        {
            File.WriteAllText(fullPath, sb.ToString());
            Debug.Log($"[DataManager] Saved: {fileName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataManager] Error saving file: {e.Message}");
        }
    }
}