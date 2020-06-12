using UnityEngine;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/Vector3")]
    public class Vector3Reference : SmartReference<Vector3> { }

    [System.Serializable]
    public class SmartVector3 : SmartVariable<Vector3, Vector3Reference> { }
}
