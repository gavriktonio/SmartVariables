using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[InitializeOnLoad]
public class SubAssetDragAndDrop
{
    static SubAssetDragAndDrop()
    {
        ProjectBrowserDragAndDrop.AddDragAndDropHandler(CustomProjectBrowserDropHandler);
    }

    internal static DragAndDropVisualMode CustomProjectBrowserDropHandler(int draggedUponID, string draggedUponPath, bool perform)
    {
        System.Type draggedOntoAssetType = AssetDatabase.GetMainAssetTypeAtPath(draggedUponPath);
        SubAssetCollection collectionAttribute = null;

        if (draggedOntoAssetType != null)
        {
            var dragOntoInfo = draggedOntoAssetType.GetTypeInfo();
            collectionAttribute = Attribute.GetCustomAttribute(dragOntoInfo, typeof(SubAssetCollection)) as SubAssetCollection;
        }

        //Dragging to collection 
        if (collectionAttribute != null)
        {
            Type[] acceptedTypes = collectionAttribute.subAssetTypes;

            //Filter out only objects of accepted types
            List<UnityEngine.Object> draggedAcceptedAssets = new List<UnityEngine.Object>(DragAndDrop.objectReferences.Where(x =>
            {
                foreach (Type type in acceptedTypes)
                {
                    if (type.IsAssignableFrom(x.GetType()))
                    {
                        return x;
                    }
                }
                return false;
            }));

            if (draggedAcceptedAssets.Count > 0)
            {
                ScriptableObject collectionToAddTo = AssetDatabase.LoadMainAssetAtPath(draggedUponPath) as ScriptableObject;
                if (collectionToAddTo == null)
                {
                    Debug.LogError("Error getting collection to add to");
                    return DragAndDropVisualMode.Rejected;
                }

                if (perform)
                {
                    MethodInfo OnAssetAddedToCollection = draggedOntoAssetType.GetMethod("OnAssetAddedToCollection");

                    foreach (ScriptableObject draggedAsset in draggedAcceptedAssets)
                    {
                        string oldPath = AssetDatabase.GetAssetPath(draggedAsset);

                        UnityEngine.Object removeFromCollection = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(draggedAsset));
                        if (removeFromCollection != null && removeFromCollection.GetType().GetCustomAttribute(typeof(SubAssetCollection)) == null)
                        {
                            removeFromCollection = null;
                        }

                        AssetDatabase.RemoveObjectFromAsset(draggedAsset);

                        if (removeFromCollection != null)
                        {
                            MethodInfo OnAssetRemovedFromCollection = removeFromCollection.GetType().GetMethod("OnAssetRemovedFromCollection");
                            if (OnAssetRemovedFromCollection != null)
                            {
                                OnAssetRemovedFromCollection.Invoke(removeFromCollection, new object[] { draggedAsset });
                            }
                        }

                        AssetDatabase.AddObjectToAsset(draggedAsset, collectionToAddTo);
                        if (AssetDatabase.LoadAllAssetsAtPath(oldPath).Length == 0)
                        {
                            AssetDatabase.DeleteAsset(oldPath);
                        }

                        OnAssetAddedToCollection.Invoke(collectionToAddTo, new object[] { draggedAsset });
                    }
                    AssetDatabase.SaveAssets();
                }
                return DragAndDropVisualMode.Move;
            }
            return DragAndDropVisualMode.Move;
        }

        //Filter out only objects that are sub assets of our collection
        List<UnityEngine.Object> draggedSubAssets = new List<UnityEngine.Object>(DragAndDrop.objectReferences.Where(x =>
        {
            return (AssetDatabase.IsSubAsset(x) && AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GetAssetPath(x)).GetCustomAttribute(typeof(SubAssetCollection)) != null);
        }));

        if (draggedSubAssets.Count > 0)
        {
            if (perform)
            {
                foreach (var draggedSubAsset in draggedSubAssets)
                {
                    UnityEngine.Object removeFromCollection = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(draggedSubAsset));
                    MethodInfo OnAssetRemovedFromCollection = removeFromCollection.GetType().GetMethod("OnAssetRemovedFromCollection");

                    AssetDatabase.RemoveObjectFromAsset(draggedSubAsset);
                    AssetDatabase.CreateAsset(draggedSubAsset, System.IO.Path.Combine(draggedUponPath, draggedSubAsset.name) + ".asset");

                    if (OnAssetRemovedFromCollection != null)
                    {
                        OnAssetRemovedFromCollection.Invoke(removeFromCollection, new object[] { draggedSubAsset });
                    }
                }
                AssetDatabase.SaveAssets();

                //if unhandled objects are left, let the leftover objects be dealt by the default method
                //Paths don't need to be touched, because DragAndDrop.paths doesn't include sub objects anyway
                if (DragAndDrop.objectReferences.Length > draggedSubAssets.Count)
                {
                    List<UnityEngine.Object> leftoverObjects = new List<UnityEngine.Object>(DragAndDrop.objectReferences.Except(draggedSubAssets));
                    DragAndDrop.objectReferences = leftoverObjects.ToArray();
                    return DragAndDropVisualMode.None;
                }
            }
            return DragAndDropVisualMode.Move;
        }

        return DragAndDropVisualMode.None;
    }
}
