using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldSetString : MonoBehaviour {

    public StringReference stringToSet;
    private InputField inputField_;
    private bool inputFieldSetFromHere = false;

    // Use this for initialization
    void Start ()
    {
        inputField_ = GetComponent<InputField>();
        inputField_.onValueChanged.AddListener(OnInputFieldChanged);
        stringToSet.AddListener(OnStringChanged);
        OnStringChanged("", stringToSet.Value);
	}
	
    void OnInputFieldChanged(string newStringValue)
    {
        if (inputFieldSetFromHere)
        {
            return;
        }
        stringToSet.Value = newStringValue;
    }

    void OnStringChanged(string oldString, string newString)
    {
        if (newString == oldString)
        {
            return;
        }
        inputFieldSetFromHere = true;
        inputField_.text = newString;
        inputFieldSetFromHere = false;

    }
}
