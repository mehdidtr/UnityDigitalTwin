using System;
using UnityEngine;

public class TurbineFilter : MonoBehaviour
{
    public static string SelectedTurbineID { get; private set; } = "All"; // Default to "All"
    public static event Action OnFilterChanged; // Event to notify listeners about filter changes

    // Method to update the selected turbine filter
    public static void UpdateFilter(string turbineID)
    {
        if (SelectedTurbineID == turbineID) return; // Avoid unnecessary updates

        SelectedTurbineID = turbineID;
        OnFilterChanged?.Invoke(); // Notify subscribers about the filter change
    }
}
