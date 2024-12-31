using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro; // For TextMeshPro

public class AverageWindSpeed : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown; // Dropdown for time intervals
    [SerializeField] private TMP_Text avgWindText; // Text placeholder to display average wind speed
    [SerializeField] private TMP_Text label; // Label to display the current turbine
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Reference to your TurbineDataContainer

    private int currentIndex; // Variable to track the current index of the dropdown
    private float avgWindSpeed; // Variable to store the average wind speed

    void Start()
    {
        if (timeDropdown == null || avgWindText == null || label == null || turbineDataContainer == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        // Subscribe to the turbine filter's event
        TurbineFilter.OnFilterChanged += OnFilterChanged;

        currentIndex = timeDropdown.selectedItemIndex;
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener to dropdown value change

        UpdateWindSpeed(); // Initial wind speed update
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
            avgWindText.text = $"{avgWindSpeed:F2} m/s"; // Format wind speed to 2 decimal places
        }
        else
        {
            avgWindText.text = "No data available";
        }
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        UpdateWindSpeed(); // Refresh wind speed calculation
        UpdateLabel(); // Refresh label
    }

    private void OnFilterChanged()
    {
        UpdateWindSpeed(); // Refresh wind speed calculation when the filter changes
        UpdateLabel(); // Refresh label
    }

    private void UpdateWindSpeed()
    {
        avgWindSpeed = 0; // Reset wind speed calculation

        if (TurbineFilter.SelectedTurbineID == "All")
        {
            // Calculate the average wind speed for all turbines at the current index
            foreach (var turbine in turbineDataContainer.turbines)
            {
                avgWindSpeed += turbine.windSpeeds[currentIndex];
            }
            avgWindSpeed /= turbineDataContainer.turbines.Length; // Average wind speed
        }
        else
        {
            // Calculate the wind speed for the selected turbine
            foreach (var turbine in turbineDataContainer.turbines)
            {
                if (turbine.turbineID == TurbineFilter.SelectedTurbineID)
                {
                    avgWindSpeed = turbine.windSpeeds[currentIndex];
                    break;
                }
            }
        }
    }

    private void UpdateLabel()
    {
        if (TurbineFilter.SelectedTurbineID == "All")
        {
            label.text = "Average Wind Speed for All Turbines";
        }
        else
        {
            label.text = $"Average Wind Speed for \"{TurbineFilter.SelectedTurbineID}\"";
        }
    }
}
