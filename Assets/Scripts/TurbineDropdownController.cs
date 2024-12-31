using System;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP; // Michsky.MUIP namespace
using TMPro;

public class TurbineDropdownController : MonoBehaviour
{
    [SerializeField] private CustomDropdown turbineDropdown; 
    [SerializeField] private TurbineDataContainer turbineDataContainer; 

    [SerializeField] private Sprite windMillUniqueIcon;
    [SerializeField] private Sprite windMillAllIcon;

    [SerializeField] private CanvasAndCameraPositionSwitcher canvasAndCameraPositionSwitcher; // Reference to the switcher

    private string[] uniqueTurbines;

    void Awake()
    {
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

        uniqueTurbines = GetUniqueTurbines();

        PopulateDropdown();
    }

    private string[] GetUniqueTurbines()
    {
        HashSet<string> turbineSet = new HashSet<string>();

        foreach (var turbine in turbineDataContainer.turbines)
        {
            turbineSet.Add(turbine.turbineID);
        }

        return new List<string>(turbineSet).ToArray();
    }

    private void PopulateDropdown()
    {
        turbineDropdown.CreateNewItem("All", windMillAllIcon, false); 
        foreach (string turbine in uniqueTurbines)
        {
            turbineDropdown.CreateNewItem(turbine, windMillUniqueIcon, false); 
        }

        turbineDropdown.SetupDropdown();

        turbineDropdown.onValueChanged.AddListener(OnDropdownOptionSelected);
    }

    private void OnDropdownOptionSelected(int selectedIndex)
    {
        string selectedTurbine = selectedIndex == 0 ? "All" : uniqueTurbines[selectedIndex - 1];

        Debug.Log("Selected turbine: " + selectedTurbine);

        TurbineFilter.UpdateFilter(selectedTurbine);

        turbineDropdown.ChangeDropdownInfo(selectedIndex);

        int cameraForTurbine = selectedIndex >= 1 && selectedIndex <= 11 ? selectedIndex : 0;

        canvasAndCameraPositionSwitcher.SwitchToView(cameraForTurbine+1); 
    }
}
