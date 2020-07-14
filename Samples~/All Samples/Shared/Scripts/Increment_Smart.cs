using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartVariables;

public class Increment_Smart : MonoBehaviour
{
    public SmartBool isIncrementing;
    public SmartFloat incrementFloat;
    public SmartFloat incrementSpeed;

    // Update is called once per frame
    void Update()
    {
        if (isIncrementing.Value == true)
        {
            incrementFloat.Value += incrementSpeed.Value * Time.deltaTime;
        }
    }
}
