using System;
using ChartAndGraph;
using UnityEngine;
using Michsky.MUIP;

public class PowerGraphic : MonoBehaviour
{
    /// <summary>
    /// Reference to the custom dropdown UI element for selecting the time interval.
    /// </summary>
    [SerializeField] private CustomDropdown timeDropdown;

    /// <summary>
    /// Reference to the ScriptableObject that contains the turbine data.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer;

    /// <summary>
    /// Reference to the GraphChart component for displaying power data.
    /// </summary>
    public GraphChart chart;

    /// <summary>
    /// Maximum number of previous power values to display in the graph.
    /// This can be configured in the Unity inspector.
    /// </summary>
    [SerializeField] private int maxValuesToDisplay = 9;

    private int currentIndex; // Track the current dropdown index
    private float currentPower = 0; // Current power value for summing

    /// <summary>
    /// Unity Start method that initializes the component, checks references, and subscribes to events.
    /// </summary>
    void Start()
    {
        // Check if all required references are assigned in the inspector
        if (timeDropdown == null || turbineDataContainer == null || chart == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        // Initialize the current index with the selected value in the dropdown
        currentIndex = timeDropdown.selectedItemIndex;

        // Add listener to the dropdown value change event
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Subscribe to the turbine filter change event
        TurbineFilter.OnFilterChanged += UpdateChart;

        // Initial chart update
        UpdateChart();
    }

    /// <summary>
    /// Unity OnDestroy method to unsubscribe from the filter change event to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from the turbine filter change event
        TurbineFilter.OnFilterChanged -= UpdateChart;
    }

    /// <summary>
    /// Event listener for dropdown value changes. Updates the chart based on the selected index.
    /// </summary>
    /// <param name="selectedIndex">The index of the selected item in the dropdown.</param>
    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        UpdateChart(); // Refresh the chart data
    }

    /// <summary>
    /// Updates the chart with power data based on the selected turbine and time interval.
    /// </summary>
    private void UpdateChart()
    {
        currentPower = 0; // Reset power calculation
        chart.DataSource.StartBatch(); // Begin batch update

        // Clear existing data points and horizontal axis mappings
        chart.DataSource.ClearCategory("Power");
        chart.HorizontalValueToStringMap.Clear();

        // Calculate the starting index based on the maximum number of values to display
        int startIndex = Mathf.Max(0, currentIndex - maxValuesToDisplay + 1);

        string filter = TurbineFilter.SelectedTurbineID; // Get the currently selected turbine filter

        // Iterate over the range of time intervals to display
        for (int i = startIndex; i <= currentIndex; i++)
        {
            // Iterate through turbines and calculate power based on the selected filter
            for (int j = 0; j < turbineDataContainer.turbines.Length; j++)
            {
                // Skip turbines that don't match the filter
                if (filter != "All" && turbineDataContainer.turbines[j].turbineID != filter)
                    continue;

                currentPower += turbineDataContainer.turbines[j].powers[i]; // Add power to the total
            }

            // Add point to the graph for the current time interval
            chart.DataSource.AddPointToCategory("Power", i, currentPower);
            currentPower = 0; // Reset power calculation

            // Map the numeric X-axis value to the original time string
            chart.HorizontalValueToStringMap[i] = turbineDataContainer.turbines[0].timeIntervals[i];
            Debug.Log($"Mapping {i} to {turbineDataContainer.turbines[0].timeIntervals[i]}");
        }

        chart.DataSource.EndBatch(); // End batch update
    }
}
