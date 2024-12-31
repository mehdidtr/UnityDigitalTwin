using System.Collections;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace for dropdown handling

public class WindmillManager : MonoBehaviour
{
    public string turbineID; // Manually set the ID in the Inspector
    public TurbineDataContainer turbineDataContainer; // Reference to TurbineDataContainer ScriptableObject
    public Transform windmillBlades; // Windmill blades to rotate
    public Vector3 rotationAxis = Vector3.forward; // Axis of rotation (default is Z-axis)
    public CustomDropdown timeDropdown; // Reference to Michsky CustomDropdown for time selection

    private TurbineData turbineData; // Holds the data for the specific turbine
    private int currentIntervalIndex = 0; // Tracks the current time interval
    private int selectedTimeFactor = 10; // Default factor in minutes

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

    // Method called when a dropdown option is selected
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

    // Parse the time factor from the interval string (assumes consistent format like "00:00-00:10")
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
        int endMinutes = int.Parse(endParts[startParts.Length - 2]);

        return endMinutes - startMinutes; // Return the difference in minutes
    }
}
