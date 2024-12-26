using System;
using System.Collections.Generic; // Ensure this is included
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PowerSumDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown timeDropdown; // Reference to the TMP_Dropdown
    [SerializeField] private TurbineDataContainer turbineDataContainer; // ScriptableObject containing turbine data
    [SerializeField] private TextMeshProUGUI powerSumLabel; // TextMeshProUGUI to display the sum of the power

    private string[] uniqueTimeIntervals;

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

    private string[] GetUniqueTimeIntervals()
    {
        HashSet<string> timeSet = new HashSet<string>();

        foreach (var turbine in turbineDataContainer.turbines)
        {
            foreach (var time in turbine.timeIntervals)
            {
                timeSet.Add(time);
            }
        }

        return new List<string>(timeSet).ToArray();
    }

    private void InitializeDropdownOptions()
    {
        timeDropdown.ClearOptions();
        timeDropdown.AddOptions(new List<string>(uniqueTimeIntervals));
    }

    private void OnDropdownValueChanged(int value)
    {
        UpdatePowerSum();
    }

    private void UpdatePowerSum()
    {
        if (powerSumLabel == null || uniqueTimeIntervals.Length == 0) return;

        string selectedTime = uniqueTimeIntervals[timeDropdown.value];
        float totalPower = 0;

        foreach (var turbine in turbineDataContainer.turbines)
        {
            int timeIndex = Array.IndexOf(turbine.timeIntervals, selectedTime);
            if (timeIndex >= 0)
            {
                totalPower += turbine.powers[timeIndex]; // Sum the power of each turbine
            }
        }

        // Display the total power sum in the TextMeshPro label
        powerSumLabel.text = $"{totalPower:F2} kW"; // Adjust the format and unit as needed
    }
}
