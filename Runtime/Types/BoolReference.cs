using UnityEngine;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/Bool")]
    public class BoolReference : SmartReference<bool> { }

    [System.Serializable]
    public class SmartBool : SmartVariable<bool, BoolReference> { }
}