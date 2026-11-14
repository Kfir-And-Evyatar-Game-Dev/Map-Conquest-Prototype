using UnityEngine;

/// <summary>Builds units at a team's base when the team can pay for them.</summary>
public class UnitFactory : MonoBehaviour
{
    [SerializeField] private Team team = Team.Blue;
    [SerializeField] private MatchSettings settings;
    [SerializeField] private ResourceBank bank;
    [SerializeField] private Region spawnRegion;
    [SerializeField, Min(0f)] private float spawnSpread = 0.8f;

    public Team Team => team;

    /// <summary>Returns the new unit, or null when the team is out of money or at the unit limit.</summary>
    public Unit TryProduce(Unit prefab)
    {
        if (CountUnits() >= settings.MaxUnitsPerTeam || !bank.TrySpend(prefab.Stats.Cost))
        {
            return null;
        }

        Vector2 position = (Vector2)spawnRegion.transform.position + Random.insideUnitCircle * spawnSpread;
        Unit unit = Instantiate(prefab, position, Quaternion.identity);
        unit.Initialize(team);
        return unit;
    }

    private int CountUnits()
    {
        int count = 0;
        foreach (Unit unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (unit.Team == team)
            {
                count++;
            }
        }

        return count;
    }
}