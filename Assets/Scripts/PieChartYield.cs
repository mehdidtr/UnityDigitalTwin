using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;
using ChartAndGraph;

public class PieChartYield : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown;
    [SerializeField] private TurbineDataContainer turbineDataContainer;
    public ChartAndGraph.PieChart chart;

    private const float maxTurbinePower = 1706f;

    [Header("Materials")]
    [SerializeField] public Material yieldMaterial;
    [SerializeField] public Material remainingMaterial;

    [Header("Colors")]
    public List<Color> hoverColors = new List<Color>();
    public List<Color> selectedColors = new List<Color>();

    private int currentIndex; // Track the current dropdown index

    private void Start()
    {
        if (timeDropdown == null || turbineDataContainer == null || chart == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        if (yieldMaterial == null || remainingMaterial == null)
        {
            Debug.LogError("Failed to load one or both materials. Please check the material names.");
            return;
        }

        // Subscribe to TurbineFilter's filter change event
        TurbineFilter.OnFilterChanged += OnFilterChanged;

        currentIndex = timeDropdown.selectedItemIndex;
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener to dropdown value change

        UpdateChart(); // Initial chart update
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        TurbineFilter.OnFilterChanged -= OnFilterChanged;
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        UpdateChart(); // Refresh chart data
    }

    private void OnFilterChanged()
    {
        UpdateChart(); // Update chart when the filter changes
    }

    private void UpdateChart()
    {
        chart.DataSource.Clear(); // Clear the existing pie chart data

        float totalPower = 0;
        int turbineCount = 0;

        // Check the selected turbine filter
        if (TurbineFilter.SelectedTurbineID == "All")
        {
            // Calculate the total power for all turbines at the currentIndex
            foreach (var turbine in turbineDataContainer.turbines)
            {
                totalPower += turbine.powers[currentIndex];
            }

            turbineCount = turbineDataContainer.turbines.Length;
        }
        else
        {
            // Calculate the power for the specific turbine
            foreach (var turbine in turbineDataContainer.turbines)
            {
                if (turbine.turbineID == TurbineFilter.SelectedTurbineID)
                {
                    totalPower += turbine.powers[currentIndex];
                    turbineCount = 1; // Only one turbine is included
                    break;
                }
            }
        }

        if (turbineCount == 0)
        {
            Debug.LogError("No turbines matched the selected filter.");
            return;
        }

        // Calculate average power per turbine
        float averagePower = totalPower / turbineCount;
        float yieldPercentage = Mathf.Clamp((averagePower / maxTurbinePower) * 100f, 0, 100);
        float remainingPercentage = 100f - yieldPercentage;

        // Assign materials to categories
        ChartDynamicMaterial yieldDynamicMaterial = new ChartDynamicMaterial(yieldMaterial, hoverColors[0], selectedColors[0]);
        ChartDynamicMaterial remainingDynamicMaterial = new ChartDynamicMaterial(remainingMaterial, hoverColors[1], selectedColors[1]);

        chart.DataSource.AddCategory("Remaining", remainingDynamicMaterial, 1.0f, 1.0f, 0.0f);
        chart.DataSource.AddCategory("Yield", yieldDynamicMaterial, 1.0f, 1.0f, 0.0f);

        chart.DataSource.SetValue("Remaining", remainingPercentage);
        chart.DataSource.SetValue("Yield", yieldPercentage);
    }
}
