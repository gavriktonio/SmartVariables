using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Collection", order = -1)]
public class SmartCollection : ScriptableObject
{
    [HideInInspector]
    public SmartReferenceBase[] variables;
}
