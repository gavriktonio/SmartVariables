
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/BoolComposite")]
    public class BoolComposite : BoolReference
    {
        [System.Serializable]
        public struct BoolCompositeElement
        {
            public  BoolReference BoolReference;
            public bool TrueWhenFalse;
        }
        public BoolCompositeElement[] BoolsToCompose;

        //Disables variable saver in parent, which is a good thing
        //because we don't want to save the value of a composite
        void OnEnable()
        {
            foreach (BoolCompositeElement boolCompositeElement in BoolsToCompose)
            {
                boolCompositeElement.BoolReference.AddListener(OnBoolChanged);
            }
            UpdateBoolValue();
        }

        private void OnBoolChanged(bool oldvalue, bool newvalue)
        {
            UpdateBoolValue();
        }

        void UpdateBoolValue()
        {
            bool composedBool = true;
            for (int i = 0; i < BoolsToCompose.Length; i++)
            {
                if (BoolsToCompose[i].TrueWhenFalse)
                {
                    composedBool = composedBool && !BoolsToCompose[i].BoolReference.Value;
                }
                else
                {
                    composedBool = composedBool && BoolsToCompose[i].BoolReference.Value;
                }
                if (!composedBool)
                {
                    break;
                }
            }
            Value = composedBool;
        }
    }
    
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BoolComposite), true)]
    public class BoolCompositeDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            //Draw bools to compose
            EditorGUI.BeginChangeCheck();
            SerializedProperty boolsToCompose = serializedObject.FindProperty("BoolsToCompose");
            EditorGUILayout.PropertyField(boolsToCompose, true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            //Draw value
            BoolComposite composite = serializedObject.targetObject as BoolComposite;
            EditorGUILayout.Toggle("Value", composite.Value);
        }
    }
#endif
}