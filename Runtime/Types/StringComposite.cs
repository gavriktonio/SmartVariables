﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(menuName = "Variables/StringComposite")]
public class StringComposite : StringReference
{
    private void OnEnable()
    {
        foreach (SmartString smartString in stringsToAppend)
        {
            if (smartString.type == VarType.Reference)
            {
                smartString.AddListener(OnStringChanged);
            }
        }
    }

    private void OnDisable()
    {
        foreach (SmartString smartString in stringsToAppend)
        {
            if (smartString.type == VarType.Reference)
            {
                smartString.RemoveListener(OnStringChanged);
            }
        }
    }

    [SerializeField]
    public SmartString[] stringsToAppend;

    void OnStringChanged(string oldString, string newString)
    {
        string composedString = "";
        for(int i = 0; i < stringsToAppend.Length; i++)
        {
            composedString = composedString + stringsToAppend[i].Value;
        }
        Value = composedString;
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(StringComposite), true)]
public class StringCompositeDrawer : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
    }
}
#endif