using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro; // For TextMeshPro

/// <summary>
/// Handles the calculation and display of average wind speed for turbines.
/// </summary>
public class AverageWindSpeed : MonoBehaviour
{
    /// <summary>
    /// Dropdown for selecting time intervals.
    /// </summary>
    [SerializeField] private CustomDropdown timeDropdown;

    /// <summary>
    /// Text placeholder to display the average wind speed.
    /// </summary>
    [SerializeField] private TMP_Text avgWindText;

    /// <summary>
    /// Label to display the current turbine information.
    /// </summary>
    [SerializeField] private TMP_Text label;

    /// <summary>
    /// Reference to the TurbineDataContainer holding turbine information.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer;

    /// <summary>
    /// Tracks the current index of the dropdown.
    /// </summary>
    private int currentIndex;

    /// <summary>
    /// Stores the calculated average wind speed.
    /// </summary>
    private float avgWindSpeed;

    /// <summary>
    /// Initializes component references, subscribes to events, and updates the UI.
    /// </summary>
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
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        UpdateWindSpeed();
        UpdateLabel();
    }

    /// <summary>
    /// Unsubscribes from events to avoid memory leaks when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        TurbineFilter.OnFilterChanged -= OnFilterChanged;
    }

    /// <summary>
    /// Updates the displayed average wind speed each frame.
    /// </summary>
    void Update()
    {
        if (turbineDataContainer != null)
        {
            avgWindText.text = $"{avgWindSpeed:F2} m/s";
        }
        else
        {
            avgWindText.text = "No data available";
        }
    }

    /// <summary>
    /// Handles the event when the dropdown value is changed.
    /// Updates the current index and refreshes the UI.
    /// </summary>
    /// <param name="selectedIndex">The new selected index of the dropdown.</param>
    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex;
        UpdateWindSpeed();
        UpdateLabel();
    }

    /// <summary>
    /// Updates the UI when the turbine filter selection changes.
    /// </summary>
    private void OnFilterChanged()
    {
        UpdateWindSpeed();
        UpdateLabel();
    }

    /// <summary>
    /// Calculates the average wind speed based on the current filter and time interval.
    /// </summary>
    private void UpdateWindSpeed()
    {
        avgWindSpeed = 0;

        if (TurbineFilter.SelectedTurbineID == "All")
        {
            // Calculate the average for all turbines
            foreach (var turbine in turbineDataContainer.turbines)
            {
                avgWindSpeed += turbine.windSpeeds[currentIndex];
            }
            avgWindSpeed /= turbineDataContainer.turbines.Length;
        }
        else
        {
            // Calculate for the selected turbine
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

    /// <summary>
    /// Updates the label text to reflect the selected turbine or "All" turbines.
    /// </summary>
    private void UpdateLabel()
    {
        if (TurbineFilter.SelectedTurbineID == "All")
        {
            label.text = "Average Wind Speed for All Turbines";
        }
        else
        {
            label.text = $"Wind Speed for \"{TurbineFilter.SelectedTurbineID}\"";
        }
    }
}
