using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadonlyAttribute))]
public sealed class ReadonlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var previousGUIState = GUI.enabled;
        // Force disabled state.
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = previousGUIState;
    }
}
