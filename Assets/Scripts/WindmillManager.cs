using System.Collections;
using UnityEngine;

public class WindmillManager : MonoBehaviour
{
    public string turbineID; // Manually set the ID in the Inspector
    public TurbineDataContainer turbineDataContainer; // Drag and drop the TurbineDataContainer ScriptableObject
    public Transform windmillBlades; // The windmill blades to rotate
    public Vector3 rotationAxis = Vector3.forward; // Axis of rotation (default is Z-axis)

    private TurbineData turbineData; // Holds the data for the specific turbine
    private int currentIntervalIndex = 0; // Tracks the current time interval

    private void Start()
    {
        // Fetch data for the turbineID from the container
        turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);

        if (turbineData != null)
        {
            Debug.Log($"Windmill {turbineData.turbineID} initialized with {turbineData.timeIntervals.Length} data entries.");
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

            // Get the current rotor speed and interval duration
            float rotorSpeed = turbineData.rotorSpeeds[currentIntervalIndex]; // In RPM
            string timeInterval = turbineData.timeIntervals[currentIntervalIndex];
            float duration = ParseTimeIntervalToSeconds(timeInterval);

            // Convert rotor speed from RPM to degrees per second
            float rotationSpeed = rotorSpeed * 6f; // 1 RPM = 6 degrees per second

            float elapsedTime = 0f;

            // Rotate the windmill blades for the duration of the current interval
            while (elapsedTime < duration)
            {
                float deltaRotation = rotationSpeed * Time.deltaTime;
                windmillBlades.Rotate(rotationAxis, deltaRotation); // Rotate around the chosen axis
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Move to the next time interval
            currentIntervalIndex = (currentIntervalIndex + 1) % turbineData.rotorSpeeds.Length;
        }
    }

    // Helper method to parse time intervals into seconds
    private float ParseTimeIntervalToSeconds(string timeInterval)
    {
        if (float.TryParse(timeInterval, out float seconds))
        {
            return seconds; // Assume the interval is already in seconds
        }

        Debug.LogWarning($"Invalid time interval format: {timeInterval}. Defaulting to 1 second.");
        return 1f; // Default to 1 second if parsing fails
    }
}