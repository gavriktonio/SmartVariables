using UnityEngine;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/Double")]
    public class DoubleReference : SmartReference<double> { }

    [System.Serializable]
    public class SmartDouble : SmartVariable<double, DoubleReference> { }
}