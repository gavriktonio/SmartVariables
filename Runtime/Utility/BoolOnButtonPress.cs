using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoolOnButtonPress : MonoBehaviour
{
    public enum BoolState
    {
        DEFAULT,
        TRUE,
        FALSE
    }
    public BoolState defaultOverride;
    public BoolReference boolToSwitch;
    public SmartString modifierButtonName;
    public SmartString buttonName;

    private void Start()
    {
        if (defaultOverride == BoolState.TRUE)
        {
            boolToSwitch.Value = true;
        }
        else if (defaultOverride == BoolState.FALSE)
        {
            boolToSwitch.Value = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
		if (Input.GetButtonDown(buttonName.Value))
        {
            if (modifierButtonName.Value == "" || Input.GetButton(modifierButtonName.Value))
            {
                boolToSwitch.Value = !boolToSwitch.Value;
            }
        }
	}
}
