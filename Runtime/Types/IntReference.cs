using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Variables/Int")]
public class IntReference : SmartReference<int> {}

[System.Serializable]
public class SmartInt : SmartVariable<int, IntReference> {}