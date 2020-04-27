using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Diagnostics.Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SubAssetCollection : Attribute
{
    public Type[] subAssetTypes;
    public SubAssetCollection(params Type[] subAssetTypes)
    {
        this.subAssetTypes = subAssetTypes;
    }
}
