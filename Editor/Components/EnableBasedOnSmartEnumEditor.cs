using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartVariables
{
    [CustomEditor(typeof(EnableBasedOnSmartEnum))]
    public sealed class EnableBasedOnSmartEnumEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SerializedProperty referenceProperty = serializedObject.FindProperty("enumReference");
            EditorGUILayout.PropertyField(referenceProperty);
            var reference = referenceProperty.objectReferenceValue as SmartReferenceBase;
            bool changed = serializedObject.ApplyModifiedProperties();

            if (reference == null)
            {
                EditorGUILayout.HelpBox("Assign an enum Smart Reference to configure a GameObject list for each value.", MessageType.Info);
                RebindIfPlaying(changed);
                return;
            }

            Type enumType = EnableBasedOnSmartEnum.GetEnumType(reference);
            if (enumType == null || !enumType.IsEnum)
            {
                EditorGUILayout.HelpBox("The assigned Smart Reference must hold an enum.", MessageType.Error);
                RebindIfPlaying(changed);
                return;
            }

            serializedObject.Update();
            SerializedProperty options = serializedObject.FindProperty("options");
            var labels = new Dictionary<string, string>();
            foreach (string name in Enum.GetNames(enumType))
            {
                string value = ((Enum)Enum.Parse(enumType, name)).ToString("D");
                string label = ObjectNames.NicifyVariableName(name);
                if (labels.ContainsKey(value))
                    labels[value] += " / " + label;
                else
                    labels.Add(value, label);
            }

            foreach (var value in labels)
            {
                SerializedProperty option = FindOption(options, enumType.FullName, value.Key);
                if (option == null)
                {
                    int index = options.arraySize;
                    options.InsertArrayElementAtIndex(index);
                    option = options.GetArrayElementAtIndex(index);
                    option.FindPropertyRelative("enumType").stringValue = enumType.FullName;
                    option.FindPropertyRelative("enumValue").stringValue = value.Key;
                    option.FindPropertyRelative("gameObjects").arraySize = 0;
                }

                EditorGUILayout.PropertyField(option.FindPropertyRelative("gameObjects"),
                    new GUIContent(value.Value), true);
            }

            // Preserve assignments if an enum member is removed so they remain reviewable.
            for (int i = 0; i < options.arraySize; i++)
            {
                SerializedProperty option = options.GetArrayElementAtIndex(i);
                string value = option.FindPropertyRelative("enumValue").stringValue;
                if (option.FindPropertyRelative("enumType").stringValue == enumType.FullName && !labels.ContainsKey(value))
                {
                    EditorGUILayout.PropertyField(option.FindPropertyRelative("gameObjects"),
                        new GUIContent("Undefined value (" + value + ")"), true);
                }
            }

            EditorGUILayout.HelpBox("Objects in the current value's list are enabled. Objects listed only for other values are disabled. Keep this component on an active object outside its target hierarchies. Assignments are preserved separately for each enum type.", MessageType.Info);
            changed |= serializedObject.ApplyModifiedProperties();
            RebindIfPlaying(changed);
        }

        private static SerializedProperty FindOption(SerializedProperty options, string enumType, string value)
        {
            for (int i = 0; i < options.arraySize; i++)
            {
                SerializedProperty option = options.GetArrayElementAtIndex(i);
                if (option.FindPropertyRelative("enumType").stringValue == enumType &&
                    option.FindPropertyRelative("enumValue").stringValue == value)
                    return option;
            }
            return null;
        }

        private void RebindIfPlaying(bool changed)
        {
            if (changed && Application.isPlaying)
                ((EnableBasedOnSmartEnum)target).Rebind();
        }
    }
}
