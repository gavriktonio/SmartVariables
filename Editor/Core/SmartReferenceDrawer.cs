using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SmartVariables
{
	[CustomPropertyDrawer(typeof(SmartReferenceBase), true)]
	public class SmartReferenceDrawer : PropertyDrawer
	{
		const float standardHeight = 16;
		const float standardGap = 2;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.objectReferenceValue == null)
			{
				return standardHeight;
			}
			else
			{
				SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
				SerializedProperty runtimeValue = serializedObject.FindProperty("runtimeValue");
				if (runtimeValue.propertyType == SerializedPropertyType.ObjectReference)
				{
					return EditorGUI.GetPropertyHeight(runtimeValue) + standardGap;
				}
				else
				{
					return EditorGUI.GetPropertyHeight(runtimeValue) + standardHeight + standardGap;
				}
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			Rect referenceRect = EditorGUI.PrefixLabel(position, label);
			referenceRect.height = standardHeight;

			Rect referencedObjectRect = new Rect(position.x, position.y + standardHeight + standardGap, position.width,
				position.height - standardHeight);

			//draw the field where you drag in the right SmartReference
			EditorGUI.PropertyField(referenceRect, property, GUIContent.none);

			if (property.objectReferenceValue != null)
			{
				SerializedObject referencedVariable = new SerializedObject(property.objectReferenceValue);
				SerializedProperty isPersistentProperty = referencedVariable.FindProperty("persistent");
				bool isVariablePersistent = isPersistentProperty.boolValue;
				SerializedProperty value;
				if (Application.isPlaying || isVariablePersistent)
				{
					value = referencedVariable.FindProperty("runtimeValue");
				}
				else
				{
					value = referencedVariable.FindProperty("initialValue");
				}

				EditorGUI.BeginChangeCheck();
				if (value.propertyType == SerializedPropertyType.ObjectReference)
				{
					EditorGUI.PropertyField(position, value, new GUIContent(" "), true);
				}
				else
				{
					EditorGUI.PropertyField(referencedObjectRect, value, new GUIContent(" "), true);
				}

				if (EditorGUI.EndChangeCheck())
				{
					if (Application.isPlaying)
					{
						//Because the editor doesn't use the value property, and sets the runtime value itself, callbacks need to be called manually
						((SmartReferenceBase) property.objectReferenceValue).PrepareEditorCallbacks();
						referencedVariable.ApplyModifiedProperties();
						((SmartReferenceBase) property.objectReferenceValue).InvokeEditorCallbacks();
					}
					else
					{
						//Don't call the callbacks when not in play mode
						referencedVariable.ApplyModifiedProperties();
					}

					EditorUtility.SetDirty(property.serializedObject.targetObject);
				}


				if (referencedVariable.FindProperty("persistent").boolValue == true)
				{
					EditorGUI.DrawRect(referenceRect, new Color(1.0f, 0.0f, 0.0f, 0.2f));
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
