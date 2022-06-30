using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
//class SmartReferenceResetter
//{
//	//Is here so that enter play mode without domain reloading works
//	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
//	static void OnRuntimeMethodLoad()
//	{
//		string[] smartReferencesGuids = AssetDatabase.FindAssets("t:" + typeof(SmartReferenceBase).Name);
//		foreach (string guid in smartReferencesGuids)
//		{
//			AssetDatabase.LoadAssetAtPath<SmartReferenceBase>(AssetDatabase.GUIDToAssetPath(guid)).RemoveAllListeners();
//		}
//		string[] smartCollectionsGuids = AssetDatabase.FindAssets("t:" + typeof(SmartCollection).Name);
//		foreach (string guid in smartCollectionsGuids)
//		{
//			SmartCollection collection = AssetDatabase.LoadAssetAtPath<SmartCollection>(AssetDatabase.GUIDToAssetPath(guid));
//			foreach (SmartReferenceBase variable in collection.variables)
//			{
//				variable.RemoveAllListeners();
//			}
//		}
//	}
//}
#endif

namespace SmartVariables
{
    public interface ISmartReferenceEnum
    {
        public string GetAsText();
        public void SetFromInt(int i);
        public int GetAsInt();
        public int GetCount();

        public virtual void SwitchToNext()
        {
            int asInt = GetAsInt();
            asInt++;
            if (asInt >= GetCount())
                asInt = 0;
            SetFromInt(asInt);
        }

        public virtual void SwitchToPrevious()
        {
            int asInt = GetAsInt();
            asInt--;
            if (asInt == -1)
                asInt = GetCount() - 1;
            SetFromInt(asInt);
        }
    }
}
