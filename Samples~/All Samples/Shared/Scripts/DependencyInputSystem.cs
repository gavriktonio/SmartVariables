using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class DependencyInputSystem : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Awake()
    {
#if !SV_SAMPLES_INPUT_SYSTEM
        text.text = "Unity Input system is required for input to work in this demo";
        text.color = Color.red;
#endif
    }
}
