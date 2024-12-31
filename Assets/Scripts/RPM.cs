using System.Collections;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro; // For TextMeshPro

public class RPM : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown; // Reference to your Michsky CustomDropdown
    [SerializeField] private TMP_Text rpmText; // Text placeholder for RPM display
    [SerializeField] private TMP_Text label; // Label to display the current turbine
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Reference to your TurbineDataContainer

    private int currentIndex; // Track the current dropdown index
    private float currentRPM = 0;

    void Start()
    {
        if (timeDropdown == null || rpmText == null || label == null || turbineDataContainer == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        // Subscribe to the turbine filter's event
        TurbineFilter.OnFilterChanged += OnFilterChanged;

        currentIndex = timeDropdown.selectedItemIndex;
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener to dropdown value change

        UpdateRPM(); // Initial RPM update
        UpdateLabel(); // Initial label update
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        TurbineFilter.OnFilterChanged -= OnFilterChanged;
    }

    void Update()
    {
        if (turbineDataContainer != null)
        {
            // Update the display with the current RPM
            rpmText.text = $"{currentRPM:F2} rpm"; // Format RPM to 2 decimal places
        }
        else
        {
            rpmText.text = "No data available";
        }
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        UpdateRPM(); // Refresh RPM calculation
        UpdateLabel(); // Refresh label
    }

    private void OnFilterChanged()
    {
        UpdateRPM(); // Refresh RPM calculation when the filter changes
        UpdateLabel(); // Refresh label
    }

    private void UpdateRPM()
    {
        currentRPM = 0; // Reset RPM calculation

        if (TurbineFilter.SelectedTurbineID == "All")
        {
            // Calculate the average RPM for all turbines at the current index
            foreach (var turbine in turbineDataContainer.turbines)
            {
                currentRPM += turbine.rotorSpeeds[currentIndex];
            }
            currentRPM /= turbineDataContainer.turbines.Length; // Average RPM
        }
        else
        {
            // Calculate the RPM for the selected turbine
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

    private void UpdateLabel()
    {
        if (TurbineFilter.SelectedTurbineID == "All")
        {
            label.text = "Average RPM for All Turbines";
        }
        else
        {
            label.text = $"Current RPM for \"{TurbineFilter.SelectedTurbineID}\"";
        }
    }
}
