using UnityEngine;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/Float")]
    public class FloatReference : SmartReference<float> { }

    [System.Serializable]
    public class SmartFloat : SmartVariable<float, FloatReference> { }
}