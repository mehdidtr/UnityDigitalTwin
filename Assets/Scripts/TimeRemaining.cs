using System.Collections;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace for CustomDropdown
using TMPro; // For TextMeshPro

public class TimeRemaining : MonoBehaviour
{
    /// <summary>
    /// Reference to the Michsky CustomDropdown UI element for selecting the time interval.
    /// </summary>
    [SerializeField] private CustomDropdown timeDropdown;

    /// <summary>
    /// Reference to the TextMeshPro UI element to display the countdown.
    /// </summary>
    [SerializeField] private TMP_Text countdownDisplay;

    /// <summary>
    /// The remaining time in the countdown.
    /// </summary>
    private float TimeLeft;

    /// <summary>
    /// Flag to control whether the countdown timer is running.
    /// </summary>
    private bool TimerOn = false;

    /// <summary>
    /// Index to track the current dropdown value.
    /// </summary>
    private int currentIndex;

    /// <summary>
    /// Unity Start method that initializes the countdown timer and listens for dropdown value changes.
    /// </summary>
    void Start()
    {
        // Initialize the current index to the first dropdown item
        currentIndex = timeDropdown.selectedItemIndex;

        // Start the countdown timer with a default duration of 10 minutes
        TimerOn = true;
        TimeLeft = 10 * 60;

        // Add listener for when the dropdown value changes
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    /// <summary>
    /// Unity Update method that handles the countdown timer logic.
    /// </summary>
    void Update()
    {
        // Ensure only one countdown timer is running at a time
        if (TimerOn)
        {
            // If there's still time left, decrease the countdown timer
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else
            {
                // Timer has finished, reset and move to the next dropdown item
                TimeLeft = 0;
                TimerOn = false;
                SelectNextDropdownValue();
            }
        }
    }

    /// <summary>
    /// Updates the countdown timer display.
    /// </summary>
    /// <param name="currentTime">The remaining time in seconds.</param>
    void updateTimer(float currentTime)
    {
        currentTime += 1; // Adjust to get an accurate display (since Time.deltaTime is subtracted in Update)

        // Calculate minutes and seconds from the remaining time
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        // Update the countdown display in the format "MM : SS"
        countdownDisplay.text = string.Format("{0:00} mins : {1:00} s", minutes, seconds);
    }

    /// <summary>
    /// Method called when the dropdown value changes. Resets the timer to 10 minutes.
    /// </summary>
    /// <param name="selectedIndex">The index of the newly selected dropdown value.</param>
    private void OnDropdownValueChanged(int selectedIndex)
    {
        // Reset the countdown timer whenever a new value is selected
        TimeLeft = 10 * 60; // 10 minutes
        TimerOn = true; // Start the timer
        Debug.Log($"Timer reset for new value: {timeDropdown.items[selectedIndex].itemName}");
        
        // Update the current dropdown index
        currentIndex = timeDropdown.selectedItemIndex;
    }

    /// <summary>
    /// Selects the next dropdown value and restarts the countdown.
    /// </summary>
    private void SelectNextDropdownValue()
    {
        // Get the number of items in the dropdown
        int itemCount = timeDropdown.items.Count; // Use the `items` property

        // Log if no items are found in the dropdown
        if (itemCount == 0)
        {
            Debug.LogWarning("Dropdown has no items!");
            return;
        }

        // Increment the index and wrap around if necessary
        if (currentIndex >= itemCount - 1)
            currentIndex = 0; // Wrap to the first item if at the end
        else
            currentIndex += 1; // Increment the index

        // Update the dropdown to show the newly selected item
        timeDropdown.ChangeDropdownInfo(currentIndex);
        Debug.Log($"CurrentIndexAfter : {currentIndex}");

        // Set the timer duration based on the new selected item
        TimeLeft = 10 * 60; // Reset the countdown to 10 minutes

        // Restart the timer
        TimerOn = true;
    }
}
