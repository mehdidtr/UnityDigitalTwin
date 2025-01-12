using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This script processes CSV data to generate turbine data and updates the TurbineDataContainer ScriptableObject.
/// It is designed to be used in the Unity Editor and processes the data to fill in the turbine data fields.
/// </summary>
public class ProcessData : MonoBehaviour
{
    /// <summary>
    /// The CSV file containing the turbine data to be processed.
    /// </summary>
    [SerializeField] private TextAsset csvFile; // Drag and drop the CSV file

    /// <summary>
    /// The target ScriptableObject that holds the turbine data.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Target ScriptableObject

    /// <summary>
    /// Generates the turbine data from the assigned CSV file and stores it in the TurbineDataContainer.
    /// </summary>
    public void GenerateTurbineData()
    {
#if UNITY_EDITOR
        // Ensure the CSV file and TurbineDataContainer are assigned
        if (csvFile == null)
        {
            Debug.LogError("No CSV file assigned!");
            return;
        }

        if (turbineDataContainer == null)
        {
            Debug.LogError("No TurbineDataContainer assigned!");
            return;
        }

        // Dictionary to group turbine data by turbine ID
        Dictionary<string, List<string[]>> turbineDataGroups = new Dictionary<string, List<string[]>>();

        // Split CSV file into lines
        string[] lines = csvFile.text.Split('\n');

        // Process lines (skip header)
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Split line into individual data columns
            string[] data = line.Split(',');
            if (data.Length < 8) continue; // Ensure all columns exist

            // Extract turbine ID and add to dictionary
            string turbineID = data[0].Trim();
            if (!turbineDataGroups.ContainsKey(turbineID))
            {
                turbineDataGroups[turbineID] = new List<string[]>();
            }
            turbineDataGroups[turbineID].Add(data);
        }

        List<TurbineData> turbinesList = new List<TurbineData>();

        // Create TurbineData objects from the grouped CSV data
        foreach (var entry in turbineDataGroups)
        {
            string turbineID = entry.Key;
            List<string[]> rows = entry.Value;

            int rowCount = rows.Count;
            string[] timeIntervals = new string[rowCount];
            int[] eventCodes = new int[rowCount];
            string[] eventCodeDescriptions = new string[rowCount];
            float[] windSpeeds = new float[rowCount];
            float[] ambientTemperatures = new float[rowCount];
            float[] rotorSpeeds = new float[rowCount];
            float[] powers = new float[rowCount];

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

            // Create a TurbineData object with the extracted data
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

        // Update the TurbineDataContainer with the newly generated turbine data
        turbineDataContainer.turbines = turbinesList.ToArray();

        // Mark the TurbineDataContainer as dirty and save the asset
        EditorUtility.SetDirty(turbineDataContainer);
        AssetDatabase.SaveAssets();

        Debug.Log("TurbineDataContainer updated successfully!");
#endif
    }
}
