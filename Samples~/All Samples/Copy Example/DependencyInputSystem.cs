using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DependencyInputSystem : MonoBehaviour
{
    static bool inputSystemImported = false;
    static DependencyInputSystem()
    {
#if SV_SAMPLES_INPUT_SYSTEM
        inputSystemImported = true;
#endif
    }

    private void OnValidate()
    {
        if (inputSystemImported == true)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
