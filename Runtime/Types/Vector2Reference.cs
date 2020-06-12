using UnityEngine;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/Vector2")]
    public class Vector2Reference : SmartReference<Vector2> { }

    [System.Serializable]
    public class SmartVector2 : SmartVariable<Vector2, Vector2Reference> { }
}