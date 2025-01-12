using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro; // For TextMeshPro

public class AverageAmbientTemp : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown; // Dropdown for time intervals
    [SerializeField] private TMP_Text avgAmbientText; // Text placeholder to display average wind speed
    [SerializeField] private TMP_Text label; // Label to display the current turbine
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Reference to your TurbineDataContainer

    private int currentIndex; // Variable to track the current index of the dropdown
    private float avgAmbientTemp; // Variable to store the average wind speed

    void Start()
    {
        if (timeDropdown == null || avgAmbientText == null || label == null || turbineDataContainer == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        // Subscribe to the turbine filter's event
        TurbineFilter.OnFilterChanged += OnFilterChanged;

        currentIndex = timeDropdown.selectedItemIndex;
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener to dropdown value change

        UpdateAmbientTemp(); // Initial wind speed update
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
            // Update the display with the current average wind speed
            avgAmbientText.text = $"{avgAmbientTemp:F2}°"; // Format wind speed to 2 decimal places
        }
        else
        {
            avgAmbientText.text = "No data available";
        }
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        UpdateAmbientTemp(); // Refresh wind speed calculation
        UpdateLabel(); // Refresh label
    }

    private void OnFilterChanged()
    {
        UpdateAmbientTemp(); // Refresh wind speed calculation when the filter changes
        UpdateLabel(); // Refresh label
    }

    private void UpdateAmbientTemp()
    {
        avgAmbientTemp = 0; // Reset wind speed calculation

        if (TurbineFilter.SelectedTurbineID == "All")
        {
            // Calculate the average wind speed for all turbines at the current index
            foreach (var turbine in turbineDataContainer.turbines)
            {
                avgAmbientTemp += turbine.ambientTemperatures[currentIndex];
            }
            avgAmbientTemp /= turbineDataContainer.turbines.Length; // Average wind speed
        }
        else
        {
            // Calculate the wind speed for the selected turbine
            foreach (var turbine in turbineDataContainer.turbines)
            {
                if (turbine.turbineID == TurbineFilter.SelectedTurbineID)
                {
                    avgAmbientTemp = turbine.ambientTemperatures[currentIndex];
                    break;
                }
            }
        }
    }

    private void UpdateLabel()
    {
        if (TurbineFilter.SelectedTurbineID == "All")
        {
            label.text = "Average Temperature for All Turbines";
        }
        else
        {
            label.text = $"Temperature for \"{TurbineFilter.SelectedTurbineID}\"";
        }
    }
}
