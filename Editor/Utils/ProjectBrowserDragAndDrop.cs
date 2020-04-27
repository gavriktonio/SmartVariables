using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;

[InitializeOnLoad]
class ProjectBrowserDragAndDrop
{
    public delegate DragAndDropVisualMode ProjectBrowserDropHandler(int draggedUponId, string draggedUponPath, bool perform);

    public static void AddDragAndDropHandler(ProjectBrowserDropHandler dropHandler)
    {
        if (customDragAndDropHandlers == null)
        {
            customDragAndDropHandlers = new List<ProjectBrowserDropHandler>();
        }
        if (!customDragAndDropHandlers.Contains(dropHandler))
        {
            customDragAndDropHandlers.Add(dropHandler);
        }
    }

    public static void RemoveDragAndDropHandler(ProjectBrowserDropHandler dropHandler)
    {
        customDragAndDropHandlers.Remove(dropHandler);
    }

    private static List<ProjectBrowserDropHandler> customDragAndDropHandlers = new List<ProjectBrowserDropHandler>(); 


    static FieldInfo draggedOntoId;
    static Assembly editorAssembly = typeof(Editor).Assembly;
    static Type dragAndDropServiceType = editorAssembly.GetType("UnityEditor.DragAndDropService");
    static MethodInfo defaultDragDropMethod = dragAndDropServiceType.GetMethod("DefaultProjectBrowserDropHandler", BindingFlags.Static | BindingFlags.NonPublic);

    static ProjectBrowserDragAndDrop()
    {
        //Assign a custom drop handler delegate
        Type projectBrowserDropHandler = dragAndDropServiceType.GetNestedType("ProjectBrowserDropHandler");

        MethodInfo handlerMethodInfo = typeof(ProjectBrowserDragAndDrop).GetMethod("CustomProjectBrowserDropHandler", BindingFlags.Static | BindingFlags.NonPublic);
        System.Delegate delegateTest = System.Delegate.CreateDelegate(projectBrowserDropHandler, handlerMethodInfo);

        MethodInfo methodInfo = dragAndDropServiceType.GetMethod("AddDropHandler", new Type[] { projectBrowserDropHandler });
        methodInfo.Invoke(null, new object[] { delegateTest });
    }

    internal static DragAndDropVisualMode CustomProjectBrowserDropHandler(int dragUponInstanceId, string dummy, bool perform)
    {
        DragAndDropVisualMode returnMode = DragAndDropVisualMode.None;
        foreach (ProjectBrowserDropHandler handler in customDragAndDropHandlers)
        {
            DragAndDropVisualMode handlerMode = handler.Invoke(dragUponInstanceId, dummy, perform);

            if (handlerMode != DragAndDropVisualMode.None && returnMode != DragAndDropVisualMode.None)
            {
                Debug.LogError("Conflict between DragAndDrop Handlers!");
                return DragAndDropVisualMode.Rejected;
            }
            returnMode = handlerMode;
        }

        if (returnMode == DragAndDropVisualMode.None)
        {
            return (DragAndDropVisualMode)defaultDragDropMethod.Invoke(null, new object[] { dragUponInstanceId, dummy, perform });
        }

        return returnMode;
    }


    ///Default Unity Project Browser Drop Handler
    //HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
    //    if (hierarchyProperty.Find(dragUponInstanceId, null))
    //    {
    //        return InternalEditorUtility.ProjectWindowDrag(hierarchyProperty, perform);
    //    }
    //    if (dragUponInstanceId != 0)
    //    {
    //        string assetPath = AssetDatabase.GetAssetPath(dragUponInstanceId);
    //        if (string.IsNullOrEmpty(assetPath))
    //        {
    //            return DragAndDropVisualMode.Rejected;
    //        }
    //        UnityEditor.PackageManager.PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(assetPath);
    //        if (packageInfo != null)
    //        {
    //            hierarchyProperty = new HierarchyProperty(packageInfo.assetPath);
    //            if (hierarchyProperty.Find(dragUponInstanceId, null))
    //            {
    //                return InternalEditorUtility.ProjectWindowDrag(hierarchyProperty, perform);
    //            }
    //        }
    //    }
    //    return InternalEditorUtility.ProjectWindowDrag(null, perform);
}