using System;
using ChartAndGraph;
using UnityEngine;
using Michsky.MUIP;

public class PowerGraphic : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown; // Reference to your Michsky CustomDropdown
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Reference to your TurbineDataContainer
    public GraphChart chart; // Reference to your GraphChart
    [SerializeField] private int maxValuesToDisplay = 9; // Maximum number of previous values to display, configurable in the editor

    private int currentIndex; // Track the current dropdown index
    private float currentPower = 0;

    void Start()
    {
        if (timeDropdown == null || turbineDataContainer == null || chart == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        currentIndex = timeDropdown.selectedItemIndex;
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener to dropdown value change

        // Subscribe to the turbine filter change event
        TurbineFilter.OnFilterChanged += UpdateChart;

        UpdateChart(); // Initial chart update
    }

    private void OnDestroy()
    {
        // Unsubscribe from the turbine filter change event to avoid memory leaks
        TurbineFilter.OnFilterChanged -= UpdateChart;
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        UpdateChart(); // Refresh chart data
    }

    private void UpdateChart()
    {
        currentPower = 0; // Reset power calculation
        chart.DataSource.StartBatch(); // Begin batch update

        // Clear existing points and horizontal axis mappings
        chart.DataSource.ClearCategory("Power");
        chart.HorizontalValueToStringMap.Clear();

        // Calculate the starting index based on the maximum values to display
        int startIndex = Mathf.Max(0, currentIndex - maxValuesToDisplay + 1);

        string filter = TurbineFilter.SelectedTurbineID; // Get the currently selected turbine filter

        for (int i = startIndex; i <= currentIndex; i++)
        {
            // Iterate through turbines and calculate power based on the filter
            for (int j = 0; j < turbineDataContainer.turbines.Length; j++)
            {
                // Skip turbines that don't match the filter
                if (filter != "All" && turbineDataContainer.turbines[j].turbineID != filter)
                    continue;

                currentPower += turbineDataContainer.turbines[j].powers[i];
            }

            // Add point to the graph
            chart.DataSource.AddPointToCategory("Power", i, currentPower);
            currentPower = 0; // Reset power calculation

            // Map the numeric X-axis value to the original time string
            chart.HorizontalValueToStringMap[i] = turbineDataContainer.turbines[0].timeIntervals[i];
            Debug.Log($"Mapping {i} to {turbineDataContainer.turbines[0].timeIntervals[i]}");
        }

        chart.DataSource.EndBatch(); // End batch update
    }
}
