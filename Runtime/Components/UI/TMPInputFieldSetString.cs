using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmartVariables
{
    public class TMPInputFieldSetString : MonoBehaviour
    {

        public StringReference stringToSet;
        private TMP_InputField inputField_;
        private bool inputFieldSetFromHere = false;

        // Use this for initialization
        void Start()
        {
            inputField_ = GetComponent<TMP_InputField>();
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
}
