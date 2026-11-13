using UnityEngine;

/// <summary>Tuning values for one kind of unit (infantry, tank...).</summary>
[CreateAssetMenu(menuName = "Map Conquest/Unit Stats")]
public class UnitStats : ScriptableObject
{
    [field: SerializeField, Min(0.1f)] public float MoveSpeed { get; private set; } = 3f;
    [field: SerializeField, Min(1)] public int MaxHealth { get; private set; } = 100;
    [field: SerializeField, Min(0)] public int Cost { get; private set; } = 20;
    [field: SerializeField] public bool CanCapture { get; private set; } = true;
    [field: SerializeField, Min(0.1f)] public float CaptureSeconds { get; private set; } = 4f;
}