using System;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro;

public class TimeDropdownController : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown; // Reference to your Michsky CustomDropdown
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Reference to your TurbineDataContainer

    [SerializeField] private Sprite schedueleIcon;

    private string[] uniqueTimeIntervals;

    void Awake()
    {
        if (turbineDataContainer == null)
        {
            Debug.LogError("TurbineDataContainer not assigned!");
            return;
        }

        // Get all unique time intervals from the turbines
        uniqueTimeIntervals = GetUniqueTimeIntervals();
        Array.Sort(uniqueTimeIntervals);

        // Populate the dropdown with the time intervals
        PopulateDropdown();
    }

    // Get all unique time intervals across all turbines
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

    // Method to populate the dropdown with options
    private void PopulateDropdown()
    {
        // Create items dynamically based on uniqueTimeIntervals
        foreach (string time in uniqueTimeIntervals)
        {
            timeDropdown.CreateNewItem(time, schedueleIcon, false); // Add new time item without icon
        }

        // Initialize the dropdown after adding items
        timeDropdown.SetupDropdown();

        // Set up a listener for when the dropdown value changes
        timeDropdown.onValueChanged.AddListener(OnDropdownOptionSelected);
    }

    // Method called when a dropdown option is selected
    private void OnDropdownOptionSelected(int selectedIndex)
    {
        // Get the selected time based on the index
        string selectedTime = uniqueTimeIntervals[selectedIndex];

        Debug.Log("Selected time: " + selectedTime);

        // Update the dropdown label to show the selected time
        timeDropdown.ChangeDropdownInfo(selectedIndex); // This updates the dropdown label to the selected item
    }
}
