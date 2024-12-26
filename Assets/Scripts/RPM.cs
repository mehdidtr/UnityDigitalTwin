using System.Collections;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro; // For TextMeshPro

public class RPM : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown; // Reference to your Michsky CustomDropdown
    [SerializeField] private TMP_Text countdownDisplay; // Text placeholder for countdown
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Reference to your TurbineDataContainer

    private int currentIndex; // Track the current dropdown index

    private float currentRPM;

    void Start()
    {
        // Ensure the dropdown value change listener is added
        if (timeDropdown != null)
        {
            currentIndex = timeDropdown.selectedItemIndex;
            timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener to dropdown value change
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the turbineDataContainer is valid and the currentIndex is within bounds
        if (turbineDataContainer != null && turbineDataContainer.turbines.Length > currentIndex)
        {
            // Assuming 'rotorSpeeds' holds the RPM values and it's indexed by the same way
            currentRPM = turbineDataContainer.turbines[currentIndex].rotorSpeeds[currentIndex];

            // Update the display with the current RPM
            countdownDisplay.text = $"{currentRPM} rpm"; // Format RPM text
        }
        else
        {
            countdownDisplay.text = "No data available";
        }
    }

    // Method to handle dropdown value change
    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        Debug.Log($"Selected dropdown index: {currentIndex}");
    }
}
