using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBasedOnBool : MonoBehaviour
{
    public BoolReference boolReference;

    public GameObject[] enableWhenTrue;
    public GameObject[] enableWhenFalse;

    public BoolReference[] trueWhenTrue;
    public BoolReference[] trueWhenFalse;
    
    void OnEnable ()
    {
        SetupListener();
	}

    private void OnDisable()
    {
        RemoveListener();
    }

    void SetupListener()
    {
        boolReference.AddListener(OnBoolChanged);
        OnBoolChanged(false, boolReference.Value);
    }

    void RemoveListener()
    {
        boolReference.RemoveListener(OnBoolChanged);
    }

    void OnBoolChanged(bool oldBool, bool newBool)
    {
        foreach (GameObject i in enableWhenTrue)
        {
            i.SetActive(newBool);
        }
        foreach (GameObject i in enableWhenFalse)
        {
            i.SetActive(!newBool);
        }
        foreach (BoolReference i in trueWhenTrue)
        {
            i.Value = newBool;
        }
        foreach (BoolReference i in trueWhenFalse)
        {
            i.Value = !newBool;
        }
    }
}
