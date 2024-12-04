using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProcessData : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile; // Drag and drop the CSV file here
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Drag and drop the TurbineDataContainer here

    public void GenerateScriptableObject()
    {
#if UNITY_EDITOR
        if (csvFile == null)
        {
            Debug.LogError("No CSV file provided. Please assign a CSV file in the Inspector.");
            return;
        }

        if (turbineDataContainer == null)
        {
            Debug.LogError("No TurbineDataContainer assigned. Please assign a TurbineDataContainer ScriptableObject.");
            return;
        }

        // Group data by turbineID
        Dictionary<string, List<string[]>> turbineDataGroups = new Dictionary<string, List<string[]>>();
        string[] lines = csvFile.text.Split('\n');

        // Skip header and process each line
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] data = line.Split(',');
            if (data.Length < 8)
            {
                Debug.LogError("Malformed data on line " + (i + 1));
                continue;
            }

            string turbineID = data[0].Trim();

            if (!turbineDataGroups.ContainsKey(turbineID))
            {
                turbineDataGroups[turbineID] = new List<string[]>();
            }
            turbineDataGroups[turbineID].Add(data);
        }

        // Create TurbineData for each turbineID and add it to TurbineDataContainer
        List<TurbineData> turbinesList = new List<TurbineData>();

        foreach (var entry in turbineDataGroups)
        {
            string turbineID = entry.Key;
            List<string[]> rows = entry.Value;

            // Prepare arrays for data
            int rowCount = rows.Count;
            string[] timeIntervals = new string[rowCount];
            int[] eventCodes = new int[rowCount];
            string[] eventCodeDescriptions = new string[rowCount];
            float[] windSpeeds = new float[rowCount];
            float[] ambientTemperatures = new float[rowCount];
            float[] rotorSpeeds = new float[rowCount];
            float[] powers = new float[rowCount];

            // Populate arrays
            for (int i = 0; i < rowCount; i++)
            {
                string[] data = rows[i];
                timeIntervals[i] = data[1].Trim();
                eventCodes[i] = int.Parse(data[2].Trim());
                eventCodeDescriptions[i] = data[3].Trim();
                windSpeeds[i] = float.Parse(data[4].Trim());
                ambientTemperatures[i] = float.Parse(data[5].Trim());
                rotorSpeeds[i] = float.Parse(data[6].Trim());
                powers[i] = float.Parse(data[7].Trim());
            }

            // Create and populate the TurbineData
            TurbineData turbineData = new TurbineData
            {
                turbineID = turbineID,
                timeIntervals = timeIntervals,
                eventCodes = eventCodes,
                eventCodeDescriptions = eventCodeDescriptions,
                windSpeeds = windSpeeds,
                ambientTemperatures = ambientTemperatures,
                rotorSpeeds = rotorSpeeds,
                powers = powers
            };

            turbinesList.Add(turbineData);
        }

        // Store the data in the TurbineDataContainer
        turbineDataContainer.turbines = turbinesList.ToArray();

        // Save the updated TurbineDataContainer asset
        EditorUtility.SetDirty(turbineDataContainer);
        AssetDatabase.SaveAssets();

        Debug.Log("TurbineDataContainer updated successfully with all turbine data!");
#endif
    }
}