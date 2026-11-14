using UnityEngine;

/// <summary>Tuning values for one match: money, income and limits.</summary>
[CreateAssetMenu(menuName = "Map Conquest/Match Settings")]
public class MatchSettings : ScriptableObject
{
    [field: SerializeField, Min(0)] public int StartingResources { get; private set; } = 60;
    [field: SerializeField, Min(0.1f)] public float IncomeIntervalSeconds { get; private set; } = 5f;
    [field: SerializeField, Min(0)] public int BaseIncome { get; private set; } = 5;
    [field: SerializeField, Min(0)] public int RegionIncome { get; private set; } = 2;
    [field: SerializeField, Min(0)] public int ResourceRegionIncome { get; private set; } = 4;
    [field: SerializeField, Min(1)] public int MaxUnitsPerTeam { get; private set; } = 12;

    [field: Header("Winning")]
    [field: SerializeField, Min(1)] public int RegionsToWin { get; private set; } = 5;
    [field: SerializeField, Min(1f)] public float HoldSecondsToWin { get; private set; } = 10f;
    [field: SerializeField, Min(10f)] public float MatchSeconds { get; private set; } = 300f;
}