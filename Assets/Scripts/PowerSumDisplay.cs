using System;
using System.Collections.Generic; // Ensure this is included
using UnityEngine;
using TMPro;

/// <summary>
/// Displays the sum of power outputs for all turbines at a selected time interval.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class PowerSumDisplay : MonoBehaviour
{
    /// <summary>
    /// Reference to the TMP_Dropdown used to select time intervals.
    /// </summary>
    [SerializeField] private TMP_Dropdown timeDropdown;

    /// <summary>
    /// ScriptableObject containing the turbine data.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer;

    /// <summary>
    /// The TextMeshProUGUI label that will display the sum of power.
    /// </summary>
    [SerializeField] private TextMeshProUGUI powerSumLabel;

    private string[] uniqueTimeIntervals;

    /// <summary>
    /// Initializes the component, setting up the dropdown and the initial power sum.
    /// </summary>
    private void Awake()
    {
        if (turbineDataContainer == null)
        {
            Debug.LogError("TurbineDataContainer not assigned!");
            return;
        }

        // Get all unique time intervals across all turbines
        uniqueTimeIntervals = GetUniqueTimeIntervals();
        Array.Sort(uniqueTimeIntervals);

        // Populate the dropdown
        InitializeDropdownOptions();

        // Add listener for dropdown value change
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Initialize the power sum
        UpdatePowerSum();
    }

    /// <summary>
    /// Retrieves all unique time intervals across all turbines.
    /// </summary>
    /// <returns>An array of unique time intervals.</returns>
    private string[] GetUniqueTimeIntervals()
    {
        HashSet<string> timeSet = new HashSet<string>();

        // Add each turbine's time intervals to the set to ensure uniqueness
        foreach (var turbine in turbineDataContainer.turbines)
        {
            foreach (var time in turbine.timeIntervals)
            {
                timeSet.Add(time);
            }
        }

        return new List<string>(timeSet).ToArray();
    }

    /// <summary>
    /// Initializes the dropdown with the unique time intervals.
    /// </summary>
    private void InitializeDropdownOptions()
    {
        timeDropdown.ClearOptions();
        timeDropdown.AddOptions(new List<string>(uniqueTimeIntervals));
    }

    /// <summary>
    /// Event handler for when the dropdown value changes.
    /// </summary>
    /// <param name="value">The selected index of the dropdown.</param>
    private void OnDropdownValueChanged(int value)
    {
        // Update the power sum when the dropdown selection changes
        UpdatePowerSum();
    }

    /// <summary>
    /// Updates the total power sum and displays it in the label.
    /// </summary>
    private void UpdatePowerSum()
    {
        if (powerSumLabel == null || uniqueTimeIntervals.Length == 0) return;

        // Get the selected time interval
        string selectedTime = uniqueTimeIntervals[timeDropdown.value];
        float totalPower = 0;

        // Sum the power outputs for each turbine at the selected time interval
        foreach (var turbine in turbineDataContainer.turbines)
        {
            int timeIndex = Array.IndexOf(turbine.timeIntervals, selectedTime);
            if (timeIndex >= 0)
            {
                totalPower += turbine.powers[timeIndex]; // Add power to the total sum
            }
        }

        // Display the total power sum in the TextMeshPro label
        powerSumLabel.text = $"{totalPower:F2} kW"; // Adjust the format and unit as needed
    }
}
