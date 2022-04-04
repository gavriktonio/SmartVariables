using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SmartVariables
{
    [CustomPropertyDrawer(typeof(SmartVariableBase), true)]
    public class SmartVariableDrawer : PropertyDrawer
    {
        const float standardHeight = 18;
        const float standardGap = 2;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("Type").enumValueIndex >= 1)
            {
                SerializedProperty referenceObjectProperty = property.FindPropertyRelative("reference");
                Object referenceObject = referenceObjectProperty.objectReferenceValue;
                if (referenceObject != null)
                {
                    //Find the runtime value in the SmartReference, referenced in the SmartVariable
                    SerializedObject serializedObject = new SerializedObject(referenceObject);
                    SerializedProperty runtimeValue = serializedObject.FindProperty("runtimeValue");

                    //Display children below only when property is opened. So no need for extra space
                    return EditorGUI.GetPropertyHeight(referenceObjectProperty, true) + standardGap;
                }
                else
                {
                    return standardHeight;
                }
            }
            else
            {
                //Find the runtime value in the SmartVariable;
                SerializedProperty runtimeValue = property.FindPropertyRelative("runtimeValue");
                return EditorGUI.GetPropertyHeight(runtimeValue, true) + standardGap;
            }
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            VarType variableType = (VarType)property.FindPropertyRelative("Type").enumValueIndex;

            Rect referenceRect = EditorGUI.PrefixLabel(position, label);

            //Draw drawer with choice of variable Type
            Rect typeSelectRect = referenceRect;
            typeSelectRect.xMin -= ((standardHeight + standardGap) + (15 * (property.depth)));
            typeSelectRect.width = standardHeight + (15 * (property.depth));

            EditorGUI.PropertyField(typeSelectRect, property.FindPropertyRelative("Type"), GUIContent.none);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            if (variableType == VarType.Constant)
            {
                SerializedProperty runtimeValue = property.FindPropertyRelative("runtimeValue");
                EditorGUI.PropertyField(position, runtimeValue, new GUIContent(" "), true);
            }

            else if (variableType == VarType.Reference)
            {
                SerializedProperty serializedProperty = property.FindPropertyRelative("reference");
                if (EditorGUI.PropertyField(position, serializedProperty, new GUIContent(" "), true))
                {
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }

            EditorGUI.EndProperty();
        }

        //Without this, the half transparent rectangles flicker (those get drawn to signify persistent references)
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }
    }
}