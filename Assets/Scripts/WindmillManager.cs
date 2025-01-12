using System.Collections;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace for dropdown handling

/// <summary>
/// Manages the behavior of a windmill, including rotor rotation and interaction with time selection via dropdown.
/// </summary>
public class WindmillManager : MonoBehaviour
{
    /// <summary>
    /// The unique ID of the turbine. This ID is manually assigned in the Inspector.
    /// </summary>
    public string turbineID;

    /// <summary>
    /// Reference to the TurbineDataContainer ScriptableObject that holds turbine data.
    /// </summary>
    public TurbineDataContainer turbineDataContainer;

    /// <summary>
    /// The Transform of the windmill blades used for rotation.
    /// </summary>
    public Transform windmillBlades;

    /// <summary>
    /// The axis of rotation for the windmill blades (default is Z-axis).
    /// </summary>
    public Vector3 rotationAxis = Vector3.forward;

    /// <summary>
    /// Reference to the CustomDropdown for selecting the time interval.
    /// </summary>
    public CustomDropdown timeDropdown;

    private TurbineData turbineData; // Holds the data for the specific turbine
    private int currentIntervalIndex = 0; // Tracks the current time interval
    private int selectedTimeFactor = 10; // Default factor in minutes

    /// <summary>
    /// Initializes the windmill, retrieves the turbine data, and starts the windmill rotation process.
    /// </summary>
    private void Start()
    {
        // Fetch data for the turbineID from the container
        turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);

        if (turbineData != null)
        {
            Debug.Log($"Windmill {turbineData.turbineID} initialized with {turbineData.timeIntervals.Length} data entries.");
            timeDropdown.onValueChanged.AddListener(OnDropdownOptionSelected);
            StartCoroutine(RotateWindmill());
        }
        else
        {
            Debug.LogError($"No TurbineData found for ID: {turbineID}");
        }
    }

    /// <summary>
    /// Rotates the windmill blades based on the rotor speed data and selected time factor.
    /// The blades rotate for the selected interval duration.
    /// </summary>
    private IEnumerator RotateWindmill()
    {
        while (true)
        {
            if (turbineData == null || turbineData.rotorSpeeds.Length == 0)
            {
                Debug.LogError("No rotor speed data available.");
                yield break;
            }

            // Use the currently selected time factor to determine duration
            float rotorSpeed = turbineData.rotorSpeeds[currentIntervalIndex]; // In RPM
            float duration = selectedTimeFactor * 60f; // Convert minutes to seconds

            // Convert rotor speed from RPM to degrees per second
            float rotationSpeed = rotorSpeed * 6f; // 1 RPM = 6 degrees per second
            float elapsedTime = 0f;

            // Rotate the windmill blades for the duration
            while (elapsedTime < duration)
            {
                float deltaRotation = rotationSpeed * Time.deltaTime;
                windmillBlades.Rotate(rotationAxis, deltaRotation); // Rotate around the chosen axis
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Move to the next time interval unless overridden by the dropdown
            if (selectedTimeFactor == 10) // Default factor
            {
                currentIntervalIndex = (currentIntervalIndex + 1) % turbineData.rotorSpeeds.Length;
            }
        }
    }

    /// <summary>
    /// Called when a dropdown option is selected to update the selected time factor.
    /// Also logs the selected turbine's RPM.
    /// </summary>
    /// <param name="selectedIndex">The index of the selected dropdown option.</param>
    private void OnDropdownOptionSelected(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= turbineData.timeIntervals.Length)
        {
            Debug.LogWarning("Invalid dropdown index selected.");
            return;
        }

        // Parse the selected time interval to extract the factor (e.g., 10 for "00:00-00:10")
        string selectedInterval = turbineData.timeIntervals[selectedIndex];
        selectedTimeFactor = ParseTimeFactor(selectedInterval);
        Debug.Log($"Selected time interval: {selectedInterval}, Factor: {selectedTimeFactor}");

        // Log the RPM for the selected turbine at this interval
        float selectedRPM = turbineData.rotorSpeeds[selectedIndex];
        Debug.Log($"Selected turbine {turbineID} RPM: {selectedRPM}");

        // Log changes for all wind turbines
        Debug.Log($"All wind turbines' RPM will be adjusted based on the selected time interval: {selectedInterval}");
    }

    /// <summary>
    /// Parses the time factor from the selected interval string (assumes a format like "00:00-00:10").
    /// </summary>
    /// <param name="timeInterval">The time interval string to parse.</param>
    /// <returns>The parsed time factor in minutes.</returns>
    private int ParseTimeFactor(string timeInterval)
    {
        string[] parts = timeInterval.Split('-');
        if (parts.Length != 2)
        {
            Debug.LogWarning($"Invalid time interval format: {timeInterval}. Defaulting to factor 10.");
            return 10; // Default to 10 minutes
        }

        string[] startParts = parts[0].Split(':');
        string[] endParts = parts[1].Split(':');

        if (startParts.Length < 2 || endParts.Length < 2)
        {
            Debug.LogWarning($"Invalid time format in interval: {timeInterval}. Defaulting to factor 10.");
            return 10;
        }

        int startMinutes = int.Parse(startParts[startParts.Length - 2]);
        int endMinutes = int.Parse(endParts[endParts.Length - 2]);

        return endMinutes - startMinutes; // Return the difference in minutes
    }
}
