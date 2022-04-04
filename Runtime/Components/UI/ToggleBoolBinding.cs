using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SmartVariables
{
    public class ToggleBoolBinding : MonoBehaviour
    {
        private Toggle toggle;
        public BoolReference boolean;

        private void OnEnable()
        {
            toggle = GetComponent<Toggle>();
            CheckReferences();
        }

        private void Start()
        {
            boolean.AddListener(OnBoolChanged);
            toggle.onValueChanged.AddListener(OnToggleChanged);

            OnBoolChanged(false, boolean.Value);
        }

        void OnBoolChanged(bool oldBool, bool newBool)
        {
            toggle.SetIsOnWithoutNotify(newBool);
        }

        void OnToggleChanged(bool newToggle)
        {
            boolean.Value = newToggle;
        }

        void CheckReferences()
        {
            if (toggle == null)
            {
                SmartLogger.LogError("A toggle component needs to be attached to this gameObject!");
            }

            if (boolean == null)
            {
                SmartLogger.LogError("A boolean variable is not assigned to use with this toggle!");
            }
        }
    }
}