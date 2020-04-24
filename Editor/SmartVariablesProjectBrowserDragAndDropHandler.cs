using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SmartVariablesProjectBrowserDragAndDropHandler
{
    static SmartVariablesProjectBrowserDragAndDropHandler()
    {
        ProjectBrowserDragAndDrop.AddDragAndDropHandler(CustomProjectBrowserDropHandler);
    }

    internal static DragAndDropVisualMode CustomProjectBrowserDropHandler(int draggedUponID, string draggedUponPath, bool perform)
    {
        System.Type draggedOntoAssetType = AssetDatabase.GetMainAssetTypeAtPath(draggedUponPath);

        object[] draggedObjects = DragAndDrop.objectReferences;
        List<SmartReferenceBase> draggedReferences = new List<SmartReferenceBase>();
        for (int i = 0; i < draggedObjects.Length; i++)
        {
            SmartReferenceBase objectAsVariable = draggedObjects[i] as SmartReferenceBase;
            if (objectAsVariable != null)
            {
                draggedReferences.Add(objectAsVariable);
            }
        }

        if (draggedReferences.Count > 0)
        {
            SmartCollection collectionToAddTo = null;
            //Dragging references on references - Create collection
            if (draggedOntoAssetType.IsSubclassOf(typeof(SmartReferenceBase)))
            {
                if (!perform)
                {
                    Debug.Log("Dragging onto smart reference");
                    return DragAndDropVisualMode.Move;
                }
                else
                {
                    SmartReferenceBase dragOntoSmartReference = AssetDatabase.LoadAssetAtPath<SmartReferenceBase>(draggedUponPath);
                    if (!draggedReferences.Contains(dragOntoSmartReference))
                    {
                        draggedReferences.Add(dragOntoSmartReference);
                    }
                    collectionToAddTo = ScriptableObject.CreateInstance<SmartCollection>();
                    AssetDatabase.CreateAsset(collectionToAddTo, Path.Combine(Path.GetDirectoryName(draggedUponPath), "Collection.asset"));
                }
            }
            //Dragging references on collection - Add to collection
            else if (draggedOntoAssetType.IsAssignableFrom(typeof(SmartCollection)))
            {
                if (!perform)
                {
                    Debug.Log("Dragging onto smart collection");
                    return DragAndDropVisualMode.Move;
                }
                else
                {
                    collectionToAddTo = AssetDatabase.LoadAssetAtPath<SmartCollection>(draggedUponPath);
                }
            }
            //Move objects into collection
            if (collectionToAddTo != null)
            {
                Undo.RecordObjects(draggedReferences.ToArray(), "Moving variables into Smart Collection");
                foreach (SmartReferenceBase smartReference in draggedReferences)
                {
                    string oldPath = AssetDatabase.GetAssetPath(smartReference);

                    AssetDatabase.RemoveObjectFromAsset(smartReference);
                    AssetDatabase.AddObjectToAsset(smartReference, collectionToAddTo);
                    if (AssetDatabase.LoadAllAssetsAtPath(oldPath).Length == 0)
                    {
                        AssetDatabase.DeleteAsset(oldPath);
                    }
                }
                AssetDatabase.SaveAssets();
                return DragAndDropVisualMode.Move;
            }

            //Dragging references into folders - Move to folders
            if (AssetDatabase.IsValidFolder(draggedUponPath))
            {
                if (!perform)
                {
                    return DragAndDropVisualMode.Move;
                }
                else
                {
                    Undo.RecordObjects(draggedReferences.ToArray(), "Moving variables into a different folder");
                    foreach (SmartReferenceBase smartReference in draggedReferences)
                    {
                        if (AssetDatabase.IsMainAsset(smartReference))
                        {
                            string oldPath = AssetDatabase.GetAssetPath(smartReference);
                            AssetDatabase.MoveAsset(oldPath, System.IO.Path.Combine(draggedUponPath, smartReference.name) + ".asset");
                        }
                        else
                        {
                            AssetDatabase.RemoveObjectFromAsset(smartReference);
                            AssetDatabase.CreateAsset(smartReference, System.IO.Path.Combine(draggedUponPath, smartReference.name) + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    return DragAndDropVisualMode.Move;
                }
            }

            return DragAndDropVisualMode.None;
        }
        return DragAndDropVisualMode.None;
    }
}
