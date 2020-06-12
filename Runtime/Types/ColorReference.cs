using UnityEngine;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/Color")]
    public class ColorReference : SmartReference<Color> { }

    [System.Serializable]
    public class SmartColor : SmartVariable<Color, ColorReference> { }
}