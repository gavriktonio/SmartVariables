using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SmartLocalCollectionWindow : EditorWindow
{
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Smart Variables Local Collections")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SmartLocalCollectionWindow));
    }
    void OnInspectorUpdate()
    {
        // Call Repaint on OnInspectorUpdate as it repaints the windows
        // less times as if it was OnGUI/Update
        Repaint();
    }

    void OnGUI()
    {
        if (Selection.activeGameObject == null)
            return;

        SmartReferenceBase selectedVariable = Selection.activeObject as SmartReferenceBase;
        if (selectedVariable != null)
        {
            Debug.Log("Tranalksnad");
        }

        List<SmartLocalCollection> visibleCollections = new List<SmartLocalCollection>();
        List<SmartLocalCollection> parentCollections = new List<SmartLocalCollection>();

        Selection.activeGameObject.GetComponentsInChildren(true, visibleCollections);
        Selection.activeGameObject.GetComponentsInParent(true, parentCollections);

        visibleCollections.AddRange(parentCollections.Where(x => !visibleCollections.Contains(x)));

        Editor collectionEditor = null;
        foreach (SmartLocalCollection collection in visibleCollections)
        {
            DrawVariableCollection(collection, ref collectionEditor);
        }
        DestroyImmediate(collectionEditor);
    }

    void DrawVariable()
    {

    }

    void DrawVariableCollection(SmartLocalCollection collection, ref Editor editor)
    {
        Editor.CreateCachedEditor(collection, typeof(SmartLocalCollectionEditor), ref editor);
        Editor.DrawFoldoutInspector(collection, ref editor);
    }
}
