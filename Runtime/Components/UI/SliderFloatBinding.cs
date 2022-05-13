using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SmartVariables
{
    public class SliderFloatBinding : MonoBehaviour
    {
        public FloatReference floatVariable;
        private Slider slider;

        private void OnEnable()
        {
            slider = GetComponent<Slider>();
            CheckReferences();

            slider.onValueChanged.AddListener(OnSliderValueChanged);
            floatVariable.AddListener(OnFloatChanged);
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            floatVariable.RemoveListener(OnFloatChanged);
        }

        private void Start()
        {
            OnFloatChanged(0, floatVariable.Value);
        }

        void OnSliderValueChanged(float newValue)
        {
            floatVariable.Value = newValue;
        }

        void OnFloatChanged(float oldValue, float newValue)
        {
            if (newValue > slider.maxValue || newValue < slider.minValue)
            {
                SmartLogger.LogWarning("Float variable is trying to set the slider out of the slider values range!");
            }

            slider.value = newValue;
        }

        void CheckReferences()
        {
            if (slider == null)
            {
                SmartLogger.LogError("A slider component needs to be attached to this gameObject!");
            }

            if (floatVariable == null)
            {
                SmartLogger.LogError("A variable is not assigned to use with this slider!");
            }
        }
    }
}
