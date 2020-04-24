using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Variables/String")]
public class StringReference : SmartReference<string> {}

[System.Serializable]
public class SmartString : SmartVariable<string, StringReference> {}