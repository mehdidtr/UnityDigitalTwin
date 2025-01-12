using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;
using ChartAndGraph;

/// <summary>
/// Updates a pie chart to display the yield and remaining potential of turbines based on the selected time range and filter.
/// </summary>
public class PieChartYield : MonoBehaviour
{
    /// <summary>
    /// Dropdown for selecting the time range.
    /// </summary>
    [SerializeField] private CustomDropdown timeDropdown;

    /// <summary>
    /// Container holding data for all turbines.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer;

    /// <summary>
    /// Reference to the pie chart being updated.
    /// </summary>
    public ChartAndGraph.PieChart chart;

    /// <summary>
    /// Maximum theoretical power output for a turbine.
    /// </summary>
    private const float maxTurbinePower = 1706f;

    [Header("Materials")]
    /// <summary>
    /// Material used for the "Yield" category in the chart.
    /// </summary>
    [SerializeField] public Material yieldMaterial;

    /// <summary>
    /// Material used for the "Remaining" category in the chart.
    /// </summary>
    [SerializeField] public Material remainingMaterial;

    [Header("Colors")]
    /// <summary>
    /// Colors used for hover effects on chart categories.
    /// </summary>
    public List<Color> hoverColors = new List<Color>();

    /// <summary>
    /// Colors used for selection effects on chart categories.
    /// </summary>
    public List<Color> selectedColors = new List<Color>();

    /// <summary>
    /// Tracks the currently selected index in the time dropdown.
    /// </summary>
    private int currentIndex;

    /// <summary>
    /// Initializes the pie chart and sets up event listeners.
    /// </summary>
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

        // Subscribe to the filter change event
        TurbineFilter.OnFilterChanged += OnFilterChanged;

        currentIndex = timeDropdown.selectedItemIndex;
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        UpdateChart(); // Initial chart update
    }

    /// <summary>
    /// Removes event listeners to avoid memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        TurbineFilter.OnFilterChanged -= OnFilterChanged;
    }

    /// <summary>
    /// Updates the chart when the dropdown value changes.
    /// </summary>
    /// <param name="selectedIndex">The selected index of the dropdown.</param>
    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex;
        UpdateChart();
    }

    /// <summary>
    /// Updates the chart when the turbine filter changes.
    /// </summary>
    private void OnFilterChanged()
    {
        UpdateChart();
    }

    /// <summary>
    /// Updates the pie chart data based on the current time range and turbine filter.
    /// </summary>
    private void UpdateChart()
    {
        chart.DataSource.Clear(); // Clear existing data

        float totalPower = 0;
        int turbineCount = 0;

        // Check the selected turbine filter
        if (TurbineFilter.SelectedTurbineID == "All")
        {
            foreach (var turbine in turbineDataContainer.turbines)
            {
                totalPower += turbine.powers[currentIndex];
            }

            turbineCount = turbineDataContainer.turbines.Length;
        }
        else
        {
            foreach (var turbine in turbineDataContainer.turbines)
            {
                if (turbine.turbineID == TurbineFilter.SelectedTurbineID)
                {
                    totalPower += turbine.powers[currentIndex];
                    turbineCount = 1;
                    break;
                }
            }
        }

        if (turbineCount == 0)
        {
            Debug.LogError("No turbines matched the selected filter.");
            return;
        }

        // Calculate yield and remaining percentages
        float averagePower = totalPower / turbineCount;
        float yieldPercentage = Mathf.Clamp((averagePower / maxTurbinePower) * 100f, 0, 100);
        float remainingPercentage = 100f - yieldPercentage;

        // Assign materials and colors to the categories
        ChartDynamicMaterial yieldDynamicMaterial = new ChartDynamicMaterial(yieldMaterial, hoverColors[0], selectedColors[0]);
        ChartDynamicMaterial remainingDynamicMaterial = new ChartDynamicMaterial(remainingMaterial, hoverColors[1], selectedColors[1]);

        chart.DataSource.AddCategory("Remaining", remainingDynamicMaterial, 1.0f, 1.0f, 0.0f);
        chart.DataSource.AddCategory("Yield", yieldDynamicMaterial, 1.0f, 1.0f, 0.0f);

        chart.DataSource.SetValue("Remaining", remainingPercentage);
        chart.DataSource.SetValue("Yield", yieldPercentage);
    }
}
