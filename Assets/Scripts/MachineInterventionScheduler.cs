using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro; // For TextMeshPro
using Michsky.MUIP; // For the dropdown

/// <summary>
/// Manages the scheduling of machine interventions for turbines and updates the displayed information based on user interactions.
/// </summary>
public class MachineInterventionScheduler : MonoBehaviour
{
    /// <summary>
    /// UI text element for displaying the intervention information.
    /// </summary>
    [SerializeField] private TMP_Text interventionValue;

    /// <summary>
    /// Container holding the data for all turbines.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer;

    /// <summary>
    /// Random number generator used to create random intervention dates.
    /// </summary>
    private System.Random random = new System.Random();

    /// <summary>
    /// Dictionary mapping turbine IDs to their scheduled intervention dates.
    /// </summary>
    private Dictionary<string, DateTime> interventionSchedule = new Dictionary<string, DateTime>();

    /// <summary>
    /// Initializes the component, schedules interventions, and sets up the initial display.
    /// </summary>
    private void Start()
    {
        if (interventionValue == null || turbineDataContainer == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        // Schedule interventions for turbines
        ScheduleInterventions();

        // Subscribe to turbine filter changes
        TurbineFilter.OnFilterChanged += UpdateLabel;

        // Update the label with the initial filter selection
        UpdateLabel();
    }

    /// <summary>
    /// Schedules random intervention dates for each turbine within a specific date range.
    /// </summary>
    private void ScheduleInterventions()
    {
        DateTime startDate = new DateTime(2021, 5, 1); // Starting date for interventions
        DateTime endDate = new DateTime(2021, 11, 1); // Ending date for interventions
        int totalDays = (endDate - startDate).Days;

        // Assign random intervention dates to each turbine
        foreach (var turbine in turbineDataContainer.turbines)
        {
            DateTime randomDate = startDate.AddDays(random.Next(0, totalDays));
            interventionSchedule[turbine.turbineID] = randomDate;
        }
    }

    /// <summary>
    /// Unsubscribes from events to prevent memory leaks when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        TurbineFilter.OnFilterChanged -= UpdateLabel;
    }

    /// <summary>
    /// Updates the displayed intervention information based on the current turbine filter selection.
    /// </summary>
    public void UpdateLabel()
    {
        string filter = TurbineFilter.SelectedTurbineID; // The currently selected turbine filter

        if (filter == "All")
        {
            // Get the earliest scheduled intervention across all turbines
            var earliestIntervention = interventionSchedule.OrderBy(kv => kv.Value).First();
            interventionValue.text = $"Next machine intervention for {earliestIntervention.Key} will happen on: {earliestIntervention.Value:MMMM dd, yyyy}";
        }
        else
        {
            // Get the scheduled intervention for the selected turbine
            if (interventionSchedule.TryGetValue(filter, out DateTime interventionDate))
            {
                interventionValue.text = $"Machine intervention for \"{filter}\" is scheduled on: {interventionDate:MMMM dd, yyyy}";
            }
            else
            {
                interventionValue.text = $"No intervention scheduled for \"{filter}\".";
            }
        }
    }
}
