using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SV_Test_IncrementFloat : MonoBehaviour
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
