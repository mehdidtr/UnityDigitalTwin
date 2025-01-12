using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro; // For TextMeshPro

/// <summary>
/// Handles the calculation and display of average ambient temperature for turbines.
/// </summary>
public class AverageAmbientTemp : MonoBehaviour
{
    /// <summary>
    /// Dropdown for selecting time intervals.
    /// </summary>
    [SerializeField] private CustomDropdown timeDropdown;

    /// <summary>
    /// Text placeholder to display the average ambient temperature.
    /// </summary>
    [SerializeField] private TMP_Text avgAmbientText;

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
    /// Stores the calculated average ambient temperature.
    /// </summary>
    private float avgAmbientTemp;

    /// <summary>
    /// Initializes component references, subscribes to events, and updates UI.
    /// </summary>
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
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        UpdateAmbientTemp();
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
    /// Updates the displayed average ambient temperature each frame.
    /// </summary>
    void Update()
    {
        if (turbineDataContainer != null)
        {
            avgAmbientText.text = $"{avgAmbientTemp:F2}°";
        }
        else
        {
            avgAmbientText.text = "No data available";
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
        UpdateAmbientTemp();
        UpdateLabel();
    }

    /// <summary>
    /// Updates the UI when the turbine filter selection changes.
    /// </summary>
    private void OnFilterChanged()
    {
        UpdateAmbientTemp();
        UpdateLabel();
    }

    /// <summary>
    /// Calculates the average ambient temperature based on the current filter and time interval.
    /// </summary>
    private void UpdateAmbientTemp()
    {
        avgAmbientTemp = 0;

        if (TurbineFilter.SelectedTurbineID == "All")
        {
            // Calculate the average for all turbines
            foreach (var turbine in turbineDataContainer.turbines)
            {
                avgAmbientTemp += turbine.ambientTemperatures[currentIndex];
            }
            avgAmbientTemp /= turbineDataContainer.turbines.Length;
        }
        else
        {
            // Calculate for the selected turbine
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

    /// <summary>
    /// Updates the label text to reflect the selected turbine or "All" turbines.
    /// </summary>
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
