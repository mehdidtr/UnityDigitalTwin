using System;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace for CustomDropdown
using TMPro;

public class TimeDropdownController : MonoBehaviour
{
    /// <summary>
    /// Reference to the Michsky CustomDropdown UI element for selecting the time interval.
    /// </summary>
    [SerializeField] private CustomDropdown timeDropdown;

    /// <summary>
    /// Reference to the ScriptableObject that contains turbine data.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer;

    /// <summary>
    /// Icon used in the dropdown items.
    /// </summary>
    [SerializeField] private Sprite schedueleIcon;

    private string[] uniqueTimeIntervals; // Array to store unique time intervals across all turbines

    /// <summary>
    /// Unity Awake method that initializes the time dropdown and populates it with unique time intervals.
    /// </summary>
    void Awake()
    {
        // Check if the TurbineDataContainer is assigned in the inspector
        if (turbineDataContainer == null)
        {
            Debug.LogError("TurbineDataContainer not assigned!");
            return;
        }

        // Get all unique time intervals from the turbine data
        uniqueTimeIntervals = GetUniqueTimeIntervals();
        Array.Sort(uniqueTimeIntervals); // Sort the time intervals

        // Populate the dropdown with the retrieved time intervals
        PopulateDropdown();
    }

    /// <summary>
    /// Gets all unique time intervals across all turbines.
    /// </summary>
    /// <returns>An array of unique time intervals as strings.</returns>
    private string[] GetUniqueTimeIntervals()
    {
        HashSet<string> timeSet = new HashSet<string>();

        // Loop through all turbines and their time intervals
        foreach (var turbine in turbineDataContainer.turbines)
        {
            foreach (var time in turbine.timeIntervals)
            {
                timeSet.Add(time); // Add unique time intervals to the set
            }
        }

        // Return the unique time intervals as an array
        return new List<string>(timeSet).ToArray();
    }

    /// <summary>
    /// Populates the dropdown with the time intervals by dynamically creating new dropdown items.
    /// </summary>
    private void PopulateDropdown()
    {
        // Create items dynamically based on uniqueTimeIntervals
        foreach (string time in uniqueTimeIntervals)
        {
            timeDropdown.CreateNewItem(time, schedueleIcon, false); // Add new time item with icon
        }

        // Initialize the dropdown after adding items
        timeDropdown.SetupDropdown();

        // Set up a listener for when the dropdown value changes
        timeDropdown.onValueChanged.AddListener(OnDropdownOptionSelected);
    }

    /// <summary>
    /// Method called when a dropdown option is selected. This updates the displayed time and logs the selected time.
    /// </summary>
    /// <param name="selectedIndex">The index of the selected dropdown option.</param>
    private void OnDropdownOptionSelected(int selectedIndex)
    {
        // Get the selected time based on the index
        string selectedTime = uniqueTimeIntervals[selectedIndex];

        // Log the selected time for debugging
        Debug.Log("Selected time: " + selectedTime);

        // Update the dropdown label to show the selected time
        timeDropdown.ChangeDropdownInfo(selectedIndex); // Update the dropdown label to the selected item
    }
}
