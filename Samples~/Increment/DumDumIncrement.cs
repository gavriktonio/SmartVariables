using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DumDumIncrement : MonoBehaviour
{
    public float incrementingFloat;
    public float incrementingSpeed;
    public bool isIncrementing;

    public Slider speedSlider;
    public TextMeshProUGUI incrementingFloatTest;
    public Toggle isIncrementingToggle;

    private void OnEnable()
    {
        speedSlider.onValueChanged.AddListener(OnSliderChanged);
        isIncrementingToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDisable()
    {
        speedSlider.onValueChanged.RemoveListener(OnSliderChanged);
        isIncrementingToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    public void Update()
    {
        if (isIncrementing)
        {
            incrementingFloat += incrementingSpeed * Time.deltaTime;
        }

        incrementingFloatTest.text = incrementingFloat.ToString();
        isIncrementingToggle.SetIsOnWithoutNotify(isIncrementing);
        speedSlider.SetValueWithoutNotify(incrementingSpeed);
    }

    void OnToggleChanged(bool newToggleValue)
    {
        isIncrementing = newToggleValue;
    }

    void OnSliderChanged(float newSliderValue)
    {
        incrementingSpeed = newSliderValue;
    }
}
