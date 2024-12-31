using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro; // For TextMeshPro
using Michsky.MUIP; // For the dropdown

public class MachineInterventionScheduler : MonoBehaviour
{
    [SerializeField] private TMP_Text interventionValue; // Label to display intervention info
    [SerializeField] private TurbineDataContainer turbineDataContainer; // Reference to your TurbineDataContainer

    private System.Random random = new System.Random();
    private Dictionary<string, DateTime> interventionSchedule = new Dictionary<string, DateTime>();

    private void Start()
    {
        if (interventionValue == null || turbineDataContainer == null)
        {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        // Schedule interventions for each turbine
        ScheduleInterventions();

        // Set up listener for turbine dropdown
        TurbineFilter.OnFilterChanged += UpdateLabel;

        // Update the label initially based on the dropdown selection
        UpdateLabel();
    }

    private void ScheduleInterventions()
    {
        DateTime startDate = new DateTime(2021, 5, 1); // May 1, 2021
        DateTime endDate = new DateTime(2021, 11, 1); // November 1, 2021
        int totalDays = (endDate - startDate).Days;

        // Generate random intervention dates for each turbine
        foreach (var turbine in turbineDataContainer.turbines)
        {
            DateTime randomDate = startDate.AddDays(random.Next(0, totalDays));
            interventionSchedule[turbine.turbineID] = randomDate;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the turbine filter change event to avoid memory leaks
        TurbineFilter.OnFilterChanged -= UpdateLabel;
    }

    public void UpdateLabel()
    {
        string filter = TurbineFilter.SelectedTurbineID; // Get the currently selected turbine filter
        if (filter == "All")
        {
            // Find the turbine with the earliest intervention
            var earliestIntervention = interventionSchedule.OrderBy(kv => kv.Value).First();
            interventionValue.text = $"Next machine intervention for {earliestIntervention.Key} will happen on: {earliestIntervention.Value:MMMM dd, yyyy}";
        }
        else
        {
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
