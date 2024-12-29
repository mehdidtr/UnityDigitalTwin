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

    [Header("Random Material Settings")]
    [SerializeField] private string materialsPath = "Materials"; // Path in Resources folder for materials
    private Material[] availableMaterials;
    private Material[] assignedMaterials; // Stores materials for each turbine

    [Header("Colors (Randomized)")]
    public List<Color> hoverColors = new List<Color>();
    public List<Color> selectedColors = new List<Color>();

    private int currentIndex; // Track the current dropdown index

    private void OnValidate()
    {
        if (turbineDataContainer != null)
        {
            LoadMaterials(); // Load materials from the specified path

            // Ensure the lists match the number of turbines in the container
            AdjustListSize(hoverColors, turbineDataContainer.turbines.Length, GenerateRandomColor());
            AdjustListSize(selectedColors, turbineDataContainer.turbines.Length, GenerateRandomColor());

            // Generate random materials once and store them
            AssignRandomMaterials();
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

    private void LoadMaterials()
    {
        availableMaterials = Resources.LoadAll<Material>(materialsPath);

        if (availableMaterials.Length == 0)
        {
            Debug.LogError($"No materials found in Resources/{materialsPath}. Ensure you have materials in the specified path.");
        }
    }

    private void AssignRandomMaterials()
    {
        if (turbineDataContainer == null) return;

        // Initialize or resize assignedMaterials array
        assignedMaterials = new Material[turbineDataContainer.turbines.Length];

        for (int i = 0; i < assignedMaterials.Length; i++)
        {
            assignedMaterials[i] = GetRandomMaterial();
        }
    }

    private Material GetRandomMaterial()
    {
        if (availableMaterials == null || availableMaterials.Length == 0)
        {
            Debug.LogError("No materials available to assign. Ensure materials are loaded properly.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, availableMaterials.Length);
        return availableMaterials[randomIndex];
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
