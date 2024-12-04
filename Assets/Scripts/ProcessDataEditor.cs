using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProcessData))]
public class ProcessDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default Inspector UI
        DrawDefaultInspector();

        // Add the button to the Inspector
        ProcessData processData = (ProcessData)target;
        if (GUILayout.Button("Generate ScriptableObject"))
        {
            processData.GenerateScriptableObject(); // Correct method name
        }
    }
}