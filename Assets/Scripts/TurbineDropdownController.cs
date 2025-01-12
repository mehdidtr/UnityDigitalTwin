using System;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace for CustomDropdown
using TMPro;

public class TurbineDropdownController : MonoBehaviour
{
    /// <summary>
    /// Reference to the Michsky CustomDropdown UI element for selecting a turbine.
    /// </summary>
    [SerializeField] private CustomDropdown turbineDropdown; 

    /// <summary>
    /// Reference to the TurbineDataContainer that holds all turbine data.
    /// </summary>
    [SerializeField] private TurbineDataContainer turbineDataContainer; 

    /// <summary>
    /// Icon for a specific turbine.
    /// </summary>
    [SerializeField] private Sprite windMillUniqueIcon;

    /// <summary>
    /// Icon for the "All" option in the dropdown.
    /// </summary>
    [SerializeField] private Sprite windMillAllIcon;

    /// <summary>
    /// Reference to the CanvasAndCameraPositionSwitcher to change camera views based on turbine selection.
    /// </summary>
    [SerializeField] private CanvasAndCameraPositionSwitcher canvasAndCameraPositionSwitcher;

    /// <summary>
    /// Array holding the unique turbine IDs.
    /// </summary>
    private string[] uniqueTurbines;

    /// <summary>
    /// Initializes the controller by loading turbine data and populating the dropdown.
    /// </summary>
    void Awake()
    {
        // Check if references are assigned
        if (turbineDataContainer == null)
        {
            Debug.LogError("TurbineDataContainer not assigned!");
            return;
        }

        if (canvasAndCameraPositionSwitcher == null)
        {
            Debug.LogError("CanvasAndCameraPositionSwitcher not assigned!");
            return;
        }

        // Get unique turbine IDs from the TurbineDataContainer
        uniqueTurbines = GetUniqueTurbines();

        // Populate the dropdown with turbine options
        PopulateDropdown();
    }

    /// <summary>
    /// Retrieves a list of unique turbine IDs from the TurbineDataContainer.
    /// </summary>
    /// <returns>A string array of unique turbine IDs.</returns>
    private string[] GetUniqueTurbines()
    {
        HashSet<string> turbineSet = new HashSet<string>();

        // Add each turbine's ID to the set (ensures uniqueness)
        foreach (var turbine in turbineDataContainer.turbines)
        {
            turbineSet.Add(turbine.turbineID);
        }

        // Convert the set to an array and return
        return new List<string>(turbineSet).ToArray();
    }

    /// <summary>
    /// Populates the dropdown with turbine options, including an "All" option and unique turbines.
    /// </summary>
    private void PopulateDropdown()
    {
        // Add "All" option at the top of the dropdown list
        turbineDropdown.CreateNewItem("All", windMillAllIcon, false); 

        // Add unique turbines to the dropdown
        foreach (string turbine in uniqueTurbines)
        {
            turbineDropdown.CreateNewItem(turbine, windMillUniqueIcon, false); 
        }

        // Setup dropdown after items are created
        turbineDropdown.SetupDropdown();

        // Add listener for dropdown value changes
        turbineDropdown.onValueChanged.AddListener(OnDropdownOptionSelected);
    }

    /// <summary>
    /// Called when a dropdown option is selected. Updates the turbine filter and camera view.
    /// </summary>
    /// <param name="selectedIndex">The index of the selected option in the dropdown.</param>
    private void OnDropdownOptionSelected(int selectedIndex)
    {
        // Determine the selected turbine ID based on dropdown index
        string selectedTurbine = selectedIndex == 0 ? "All" : uniqueTurbines[selectedIndex - 1];

        // Log the selected turbine for debugging purposes
        Debug.Log("Selected turbine: " + selectedTurbine);

        // Update the turbine filter with the selected turbine ID
        TurbineFilter.UpdateFilter(selectedTurbine);

        // Update the dropdown label to reflect the selected item
        turbineDropdown.ChangeDropdownInfo(selectedIndex);

        // Determine the camera view based on the selected turbine index
        // If index is within range, show specific turbine camera, otherwise show default camera
        int cameraForTurbine = selectedIndex >= 1 && selectedIndex <= 11 ? selectedIndex : 0;

        // Switch to the appropriate camera view
        canvasAndCameraPositionSwitcher.SwitchToView(cameraForTurbine + 1); 
    }
}
