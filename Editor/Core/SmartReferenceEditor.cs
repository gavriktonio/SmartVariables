using UnityEngine;
using UnityEditor;

namespace SmartVariables
{
    //Shows when a smart reference object is selected in assets
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SmartReferenceBase), true)]
    public class SmartReferenceEditor : Editor
    {
        protected SerializedProperty debugLogProperty_;
        protected SerializedProperty persistentProperty_;
        protected SerializedProperty variableSaverProperty_;
        protected SerializedProperty forceCallbacksProperty_;
        protected SerializedProperty InitialValueProperty_;
        protected SerializedProperty RuntimeValueProperty_;
        protected SerializedProperty NameProperty_;

        void OnEnable()
        {
            // I think it's a unity error that a null error sometimes happens here
            try
            {
                if (serializedObject == null)
                {
                    return;
                }

                debugLogProperty_ = serializedObject.FindProperty("DebugLog");
                persistentProperty_ = serializedObject.FindProperty("Persistent");
                variableSaverProperty_ = serializedObject.FindProperty("VariableSaver");
                forceCallbacksProperty_ = serializedObject.FindProperty("ForceCallbacks");
                InitialValueProperty_ = serializedObject.FindProperty("initialValue");
                RuntimeValueProperty_ = serializedObject.FindProperty("runtimeValue");
            }
            catch { }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(serializedObject.targetObject.GetType().ToString(), EditorStyles.boldLabel);
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(debugLogProperty_);
            EditorGUILayout.PropertyField(persistentProperty_);
            EditorGUILayout.PropertyField(variableSaverProperty_);
            EditorGUILayout.PropertyField(forceCallbacksProperty_);
            if (InitialValueProperty_ != null)
                EditorGUILayout.PropertyField(InitialValueProperty_, true);
            if (RuntimeValueProperty_ != null)
                EditorGUILayout.PropertyField(RuntimeValueProperty_, true);

            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                //Because the editor doesn't use the value property, and sets the runtime value itself, callbacks need to be called manually
                ((SmartReferenceBase)serializedObject.targetObject).PrepareEditorCallbacks();
                serializedObject.ApplyModifiedProperties();
                ((SmartReferenceBase)serializedObject.targetObject).InvokeEditorCallbacks();
            }
            else
            {
                //Don't call the callbacks if not in play mode
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    public class SmartReferenceLocalEditor : SmartReferenceEditor
    {
        private void NameField()
        {
            string inputName = EditorGUILayout.TextField(target.name);
            if (inputName != target.name)
            {
                Undo.RecordObject(target, "Edit Smart Reference name");
                target.name = inputName;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(serializedObject.targetObject.GetType().ToString(), EditorStyles.boldLabel);
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            NameField();

            if (Application.isPlaying)
            {
                EditorGUILayout.PropertyField(RuntimeValueProperty_, true);
            }
            else
            {
                EditorGUILayout.PropertyField(InitialValueProperty_, true);
            }

            EditorGUILayout.PropertyField(debugLogProperty_);
            EditorGUILayout.PropertyField(forceCallbacksProperty_);

            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                //Because the editor doesn't use the value property, and sets the runtime value itself, callbacks need to be called manually
                ((SmartReferenceBase)serializedObject.targetObject).PrepareEditorCallbacks();
                serializedObject.ApplyModifiedProperties();
                ((SmartReferenceBase)serializedObject.targetObject).InvokeEditorCallbacks();
            }
            else
            {
                //Don't call the callbacks if not in play mode
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}