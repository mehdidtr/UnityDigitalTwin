using System.Collections;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro; // For TextMeshPro

public class AverageWindSpeed : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown; // Fait référence au menu déroulant du Time Selector
    [SerializeField] private TMP_Text avgWindText; // Le placeholder de type texte qui affichera la valeur à l'écran
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Reference to your TurbineDataContainer

    private int currentIndex; // Variable pour suivre l'index actuel du menu déroulant

    private float avgWindSpeed; // Variable pour stocker la vitesse moyenne du vent

    void Start()
    {
        if (timeDropdown != null)
        {
            currentIndex = timeDropdown.selectedItemIndex;
            timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged); // Add listener to dropdown value change
            for (int i = 0; i < turbineDataContainer.turbines.Length; i++)
            {
                avgWindSpeed += turbineDataContainer.turbines[i].windSpeeds[currentIndex];
            }
            avgWindSpeed /= turbineDataContainer.turbines.Length; // Calculate the average wind speed
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (turbineDataContainer != null)
        {
            // Update the display with the current RPM
            avgWindText.text = $"{avgWindSpeed} m/s"; // Format RPM text
        }
        else
        {
            avgWindText.text = "No data available";
        }
    }

    // Method to handle dropdown value change
    private void OnDropdownValueChanged(int selectedIndex)
    {
        currentIndex = selectedIndex; // Update the current index when the dropdown value changes
        avgWindSpeed = 0;
        for (int i = 0; i < turbineDataContainer.turbines.Length; i++)
        {
            avgWindSpeed += turbineDataContainer.turbines[i].windSpeeds[currentIndex];
        }
        avgWindSpeed /= turbineDataContainer.turbines.Length; // Calculate the average wind speed
        //Debug.Log($"Selected dropdown index: {currentIndex}");
    }
}
