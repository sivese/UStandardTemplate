using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Std.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RatioScaler))]
    public class RatioScalerEditor : Editor
    {
        private SerializedProperty widthRatio;
        private SerializedProperty heightRatio;

        private GUIContent editorIcon;

        private void OnEnable()
        {
            //editorIcon = new("");
            //editorIcon;

            widthRatio = serializedObject.FindProperty("widthRatio");
            heightRatio = serializedObject.FindProperty("heightRatio");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(widthRatio);
            EditorGUILayout.PropertyField(heightRatio);

            serializedObject.ApplyModifiedProperties();
        }
    }
}