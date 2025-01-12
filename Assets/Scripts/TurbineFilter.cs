using System;
using UnityEngine;

public class TurbineFilter : MonoBehaviour
{
    /// <summary>
    /// The ID of the currently selected turbine. Default is "All".
    /// </summary>
    public static string SelectedTurbineID { get; private set; } = "All";

    /// <summary>
    /// Event that is triggered when the turbine filter is changed.
    /// Subscribers will be notified about filter changes.
    /// </summary>
    public static event Action OnFilterChanged;

    /// <summary>
    /// Updates the selected turbine filter with the given turbine ID.
    /// If the turbine ID is the same as the current one, no update is made.
    /// </summary>
    /// <param name="turbineID">The ID of the turbine to set as the new filter.</param>
    public static void UpdateFilter(string turbineID)
    {
        // Avoid unnecessary updates if the selected turbine ID is the same as the current one
        if (SelectedTurbineID == turbineID) return;

        // Update the selected turbine ID
        SelectedTurbineID = turbineID;

        // Trigger the OnFilterChanged event to notify subscribers of the change
        OnFilterChanged?.Invoke();
    }
}
