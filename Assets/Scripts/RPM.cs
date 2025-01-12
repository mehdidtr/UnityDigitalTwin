using System.Collections;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace for CustomDropdown
using TMPro; // For TextMeshPro

public class RPM : MonoBehaviour
{
    /// <summary>
    /// Reference to the Michsky CustomDropdown UI element for selecting the time interval.
    /// </summary>
    [SerializeField] private CustomDropdown timeDropdown;

    /// <summary>
    /// TextMeshPro UI text field to display the current RPM.
    /// </summary>
    [SerializeField] private TMP_Text rpmText;

    /// <summary>
    /// Label that displays the name of the current turbine or "All Turbines".
    /// </summary>
    [SerializeField] private TMP_Text label;

    /// <summary>
    /// Reference to the ScriptableObject that contains turbine data.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer;

    private int currentIndex; // Track the current dropdown index
    private float currentRPM = 0; // Current RPM value

    /// <summary>
    /// Unity Start method that initializes the component, subscribes to events, and performs the initial update.
    /// </summary>
    void Start()
    {
        // Check if all required references are assigned in the inspector
        if (timeDropdown == null || rpmText == null || label == null || turbineDataContainer == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        // Subscribe to the turbine filter's event for when the selected turbine changes
        TurbineFilter.OnFilterChanged += OnFilterChanged;

        // Initialize the current index with the selected value in the dropdown
        currentIndex = timeDropdown.selectedItemIndex;

        // Add listener to handle dropdown value changes
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Perform the initial RPM and label update
        UpdateRPM();
        UpdateLabel();
    }

    /// <summary>
    /// Unity OnDestroy method to unsubscribe from events and prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from the turbine filter's change event
        TurbineFilter.OnFilterChanged -= OnFilterChanged;
    }

    /// <summary>
    /// Unity Update method that updates the RPM display each frame.
    /// </summary>
    void Update()
    {
        if (turbineDataContainer != null)
        {
            // Display the current RPM value, formatted to two decimal places
            rpmText.text = $"{currentRPM:F2} rpm";
        }
        else
        {
            rpmText.text = "No data available"; // Display a message if no data is available
        }
    }

    /// <summary>
    /// Event listener for dropdown value changes. Updates the current index and refreshes the RPM and label.
    /// </summary>
    /// <param name="selectedIndex">The index of the selected item in the dropdown.</param>
    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        UpdateRPM(); // Refresh RPM calculation
        UpdateLabel(); // Refresh the label text
    }

    /// <summary>
    /// Event listener for changes to the turbine filter. Updates the RPM and label when the selected turbine changes.
    /// </summary>
    private void OnFilterChanged()
    {
        UpdateRPM(); // Refresh RPM calculation based on the filter
        UpdateLabel(); // Refresh the label text based on the filter
    }

    /// <summary>
    /// Calculates the RPM based on the selected time interval and turbine filter.
    /// </summary>
    private void UpdateRPM()
    {
        currentRPM = 0; // Reset the RPM calculation

        if (TurbineFilter.SelectedTurbineID == "All")
        {
            // Calculate the average RPM for all turbines at the current time index
            foreach (var turbine in turbineDataContainer.turbines)
            {
                currentRPM += turbine.rotorSpeeds[currentIndex];
            }
            currentRPM /= turbineDataContainer.turbines.Length; // Average RPM
        }
        else
        {
            // Calculate the RPM for the selected turbine at the current time index
            foreach (var turbine in turbineDataContainer.turbines)
            {
                if (turbine.turbineID == TurbineFilter.SelectedTurbineID)
                {
                    currentRPM = turbine.rotorSpeeds[currentIndex];
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Updates the label text to reflect the currently selected turbine or "All Turbines".
    /// </summary>
    private void UpdateLabel()
    {
        if (TurbineFilter.SelectedTurbineID == "All")
        {
            label.text = "Average RPM for All Turbines"; // Label for all turbines
        }
        else
        {
            label.text = $"Current RPM for \"{TurbineFilter.SelectedTurbineID}\""; // Label for a specific turbine
        }
    }
}
