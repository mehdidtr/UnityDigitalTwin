using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for the <c>ProcessData</c> script, adding a button in the Unity Inspector to generate turbine data.
/// </summary>
[CustomEditor(typeof(ProcessData))]
public class ProcessDataEditor : Editor
{
    /// <summary>
    /// Overrides the default Inspector GUI to include a custom button for generating turbine data.
    /// </summary>
    public override void OnInspectorGUI()
    {
        // Draw the default Inspector elements
        DrawDefaultInspector();

        // Get the ProcessData instance targeted by this editor
        ProcessData processData = (ProcessData)target;

        // Add a custom button to the Inspector
        if (GUILayout.Button("Generate Turbine Data"))
        {
            // Calls the GenerateTurbineData method in the ProcessData script when the button is clicked
            processData.GenerateTurbineData();
        }
    }
}
