using System;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;
using ChartAndGraph;

[ExecuteAlways] // Ensures the script updates the Inspector in edit mode
public class PieChartPowerTurbines : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown;
    [SerializeField] private TurbineDataContainer turbineDataContainer;
    public ChartAndGraph.PieChart chart;
    [SerializeField] private int maxValuesToDisplay = 9;

    [Header("Materials for Turbines")]
    [SerializeField] private Material[] assignedMaterials; // Manually assign materials in the Inspector

    [Header("Colors (Randomized)")]
    public List<Color> hoverColors = new List<Color>();
    public List<Color> selectedColors = new List<Color>();

    private int currentIndex; // Track the current dropdown index

    private void OnValidate()
    {
        if (turbineDataContainer != null)
        {
            // Ensure the lists match the number of turbines in the container
            AdjustListSize(hoverColors, turbineDataContainer.turbines.Length, GenerateRandomColor());
            AdjustListSize(selectedColors, turbineDataContainer.turbines.Length, GenerateRandomColor());

            // Validate or initialize materials array
            AdjustMaterialsArraySize();
        }
    }

    private void Start()
    {
        if (timeDropdown == null || turbineDataContainer == null || chart == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        currentIndex = timeDropdown.selectedItemIndex;
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener to dropdown value change

        UpdateChart(); // Initial chart update
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        UpdateChart(); // Refresh chart data
    }

    private void UpdateChart()
    {
        chart.DataSource.Clear(); // Clear the existing pie chart data

        Dictionary<string, float> turbinePowers = new Dictionary<string, float>();
        int startIndex = Mathf.Max(0, currentIndex - maxValuesToDisplay + 1);
        float totalPower = 0;

        // Calculate power for each turbine over the selected range
        for (int j = 0; j < turbineDataContainer.turbines.Length; j++)
        {
            float turbineTotal = 0;

            for (int i = startIndex; i <= currentIndex; i++)
            {
                turbineTotal += turbineDataContainer.turbines[j].powers[i];
            }

            string turbineName = turbineDataContainer.turbines[j].turbineID;
            turbinePowers[turbineName] = turbineTotal;
            totalPower += turbineTotal;
        }

        // Update the pie chart with calculated power and assign stored materials and colors
        for (int i = 0; i < turbineDataContainer.turbines.Length; i++)
        {
            string turbineName = turbineDataContainer.turbines[i].turbineID;
            if (!turbinePowers.ContainsKey(turbineName)) continue;

            float percentage = totalPower > 0 ? (turbinePowers[turbineName] / totalPower) * 100f : 0;

            if (!chart.DataSource.HasCategory(turbineName))
            {
                // Use assigned material and colors
                Material material = assignedMaterials[i];
                Color hover = hoverColors[i];
                Color selected = selectedColors[i];
                ChartDynamicMaterial dynamicMaterial = new ChartDynamicMaterial(material, hover, selected);

                chart.DataSource.AddCategory(turbineName, dynamicMaterial, 1.0f, 1.0f, 0.0f);
            }

            chart.DataSource.SetValue(turbineName, percentage);
        }
    }

    private void AdjustMaterialsArraySize()
    {
        if (turbineDataContainer == null) return;

        if (assignedMaterials == null || assignedMaterials.Length != turbineDataContainer.turbines.Length)
        {
            Material[] newMaterials = new Material[turbineDataContainer.turbines.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = i < assignedMaterials?.Length ? assignedMaterials[i] : null;
            }

            assignedMaterials = newMaterials;
        }
    }

    private Color GenerateRandomColor()
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }

    private void AdjustListSize<T>(List<T> list, int targetSize, T defaultValue)
    {
        // Add or remove elements to match the target size
        while (list.Count < targetSize)
        {
            list.Add(defaultValue);
        }
        while (list.Count > targetSize)
        {
            list.RemoveAt(list.Count - 1);
        }
    }
}
