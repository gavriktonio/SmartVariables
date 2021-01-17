using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Reflection;
#endif
namespace SmartVariables
{
    [CreateAssetMenu(menuName = "Variables/Collection", order = -1)]
    [SubAssetCollection(typeof(SmartReferenceBase), typeof(SmartVariableSaverBase))]
    public class SmartCollection : ScriptableObject
    {
        public List<SmartReferenceBase> variables = new List<SmartReferenceBase>();
        public SmartVariableSaverBase variableSaver;

        public void OnAssetAddedToCollection(Object asset)
        {
#if UNITY_EDITOR
            SmartReferenceBase addedVariable = asset as SmartReferenceBase;
            if (addedVariable != null)
            {
                variables.Add(addedVariable);
                if (variableSaver != null)
                {
                    addedVariable.VariableSaver = variableSaver;
                }

                return;
            }

            SmartVariableSaverBase addedVariableSaver = asset as SmartVariableSaverBase;
            if (addedVariableSaver != null)
            {
                if (variableSaver != null)
                {
                }
                else
                {
                    variableSaver = addedVariableSaver;
                    foreach (SmartReferenceBase variable in variables)
                    {
                        variable.VariableSaver = addedVariableSaver;
                    }
                }

                return;
            }
#endif
        }

        public void OnAssetRemovedFromCollection(Object asset)
        {
#if UNITY_EDITOR
            SmartVariableSaverBase removedVariableSaver = asset as SmartVariableSaverBase;
            if (removedVariableSaver != null)
            {
                foreach (SmartReferenceBase variable in variables)
                {
                    variable.VariableSaver = null;
                }

                variableSaver = null;
                return;
            }

            SmartReferenceBase removedVariable = asset as SmartReferenceBase;
            if (removedVariable != null)
            {
                removedVariable.VariableSaver = null;
                variables.Remove(removedVariable);
                return;
            }
#endif
        }
    }

#if UNITY_EDITOR
//Here to fix a unity bug https://issuetracker.unity3d.com/issues/parent-and-child-nested-scriptable-object-assets-switch-places-when-parent-scriptable-object-asset-is-renamed
    public class CustomAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        static private bool isWithinRename = false;

        //Just using AssetDatabase.RenameAsset causes a bug where one of the subAssets might become the main asset
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            //We are only concerned with renames
            if (isWithinRename || Path.GetDirectoryName(sourcePath) != Path.GetDirectoryName(destinationPath))
                return AssetMoveResult.DidNotMove;
            isWithinRename = true;

            SubAssetCollection collectionAttribute =
                AssetDatabase.GetMainAssetTypeAtPath(sourcePath).GetCustomAttribute(typeof(SubAssetCollection)) as
                    SubAssetCollection;
            if (collectionAttribute != null)
            {
                Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(sourcePath);

                foreach (Object obj in subAssets)
                {
                    AssetDatabase.RemoveObjectFromAsset(obj);
                }

                AssetDatabase.RenameAsset(sourcePath, Path.GetFileNameWithoutExtension(destinationPath));
                foreach (Object obj in subAssets)
                {
                    AssetDatabase.AddObjectToAsset(obj, destinationPath);
                }

                AssetDatabase.SaveAssets();

                isWithinRename = false;
                return AssetMoveResult.DidMove;
            }

            isWithinRename = false;
            return AssetMoveResult.DidNotMove;
        }
    }
#endif
}