using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorPickerBecauseWhyNot))]
public class ColorPickerBecauseWhyNotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ColorPickerBecauseWhyNot script = (ColorPickerBecauseWhyNot)target;
        if (GUILayout.Button("Change Color"))
        {
            script.ChangeColor();
        }
    }
}