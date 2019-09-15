using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Example2_2;

[CustomEditor(typeof(CircleImage), true)]
[CanEditMultipleObjects]
public class CircleImageEditor : UnityEditor.UI.ImageEditor
{
    SerializedProperty _fillPercent;
    SerializedProperty _segements;

    protected override void OnEnable()
    {
        base.OnEnable();
        _fillPercent = serializedObject.FindProperty("fillPercent");
        _segements = serializedObject.FindProperty("segementCount");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.Slider(_fillPercent, 0, 1, new GUIContent("fillPercent"));

        EditorGUILayout.PropertyField(_segements);

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }
}
