using System.Collections;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro; // For TextMeshPro

public class TimeRemaining : MonoBehaviour
{
    [SerializeField] private CustomDropdown timeDropdown; // Reference to your Michsky CustomDropdown
    [SerializeField] private TMP_Text countdownDisplay; // Text placeholder for countdown
    
    private float TimeLeft;
    private bool TimerOn = false;
    private int currentIndex; // Track the current dropdown index

    void Start()
    {
        if (timeDropdown == null || countdownDisplay == null)
        {
            Debug.LogError("TimeDropdown or CountdownDisplay not assigned!");
            return;
        }

        // Start with the first dropdown value
        currentIndex = 0;
        TimerOn = true;
        TimeLeft = 1 * 60;
        timeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Method to start the countdown for the selected dropdown item
    void Update()
    {
        // Ensure no multiple coroutines are running
        if (TimerOn){
            if (TimeLeft > 0){
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else{
                TimeLeft = 0;
                TimerOn = false;
                SelectNextDropdownValue();
            }
        }
  }

    // Coroutine to handle the countdown with real-time updates
    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        countdownDisplay.text = string.Format("{0:00} mins : {1:00 s}",minutes , seconds); // Display both minutes and seconds

        // When the timer hits 0, select the next dropdown value
        
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        // Reset the timer to 10 minutes (600 seconds) whenever a new value is selected
        TimeLeft = 1 * 60; // 10 minutes
        TimerOn = true; // Start the timer
        Debug.Log($"Timer reset for new value: {timeDropdown.items[selectedIndex].itemName}");
    }
    // Select the next dropdown value and restart the countdown
    private void SelectNextDropdownValue()
    {
        // Get the number of items in the dropdown
        int itemCount = timeDropdown.items.Count; // Use the `items` property

        if (itemCount == 0)
        {
            Debug.LogWarning("Dropdown has no items!");
            return;
        }

        // Increment the index and wrap around if necessary
        currentIndex = (currentIndex + 1) % itemCount;

        // Update the dropdown's selected index
        timeDropdown.ChangeDropdownInfo(currentIndex);

        // Set the timer duration based on the new selected item
        TimeLeft = 1 * 60;

        // Restart the timer
        TimerOn = true;
    }
}
