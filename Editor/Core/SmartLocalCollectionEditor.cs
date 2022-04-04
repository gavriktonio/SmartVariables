using UnityEngine;
using UnityEditor;

namespace SmartVariables
{
    [CustomEditor(typeof(SmartLocalCollection))]
    public class SmartLocalCollectionEditor : Editor
    {
        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            SmartLocalCollection collection = target as SmartLocalCollection;

            //Drag in variable to add
            Object draggedInObject = EditorGUILayout.ObjectField("Add To Collection", null, typeof(SmartReferenceBase), false);
            SmartReferenceBase draggedInVariable = draggedInObject as SmartReferenceBase;
            if (draggedInVariable != null)
            {
                collection.AddToCollection(draggedInVariable);
            }

            //Draw foldout editors for variables
            Editor editor = null;
            foreach (SmartReferenceBase variable in collection.variables)
            {
                Editor.CreateCachedEditor(variable, typeof(SmartReferenceLocalEditor), ref editor);
                DrawFoldoutInspector(variable, ref editor);
            }
            DestroyImmediate(editor);
        }
    }
}

