using UnityEngine;

/// <summary>
/// A simple computer opponent: every few seconds it builds the next unit in its build order
/// (if it can pay) and sends each idle unit to the nearest region it doesn't own yet.
/// </summary>
public class EnemyCommander : MonoBehaviour
{
    [SerializeField] private Team team = Team.Red;
    [SerializeField] private UnitFactory factory;
    [SerializeField] private Unit[] buildOrder;
    [SerializeField, Min(0.5f)] private float secondsBetweenDecisions = 3f;
    [SerializeField, Min(0.1f)] private float arrivedDistance = 0.6f;

    private Region[] regions;
    private int nextBuildIndex;
    private float secondsUntilDecision;

    private void Awake()
    {
        regions = FindObjectsByType<Region>(FindObjectsSortMode.None);
        secondsUntilDecision = secondsBetweenDecisions;
    }

    private void Update()
    {
        secondsUntilDecision -= Time.deltaTime;
        if (secondsUntilDecision > 0f)
        {
            return;
        }

        secondsUntilDecision += secondsBetweenDecisions;
        TryBuild();
        SendIdleUnits();
    }

    private void TryBuild()
    {
        if (buildOrder.Length == 0)
        {
            return;
        }

        if (factory.TryProduce(buildOrder[nextBuildIndex]) != null)
        {
            nextBuildIndex = (nextBuildIndex + 1) % buildOrder.Length;
        }
    }

    private void SendIdleUnits()
    {
        foreach (Unit unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (unit.Team != team || !unit.TryGetComponent(out UnitMover mover) || mover.IsMoving)
            {
                continue;
            }

            Region target = NearestRegionToTake(unit.transform.position);
            if (target != null && Vector2.Distance(unit.transform.position, target.transform.position) > arrivedDistance)
            {
                mover.MoveTo(target.transform.position);
            }
        }
    }

    // The closest non-base region this team doesn't own. A unit already standing in it stays there to capture it.
    private Region NearestRegionToTake(Vector2 from)
    {
        Region nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Region region in regions)
        {
            if (region.IsBase || region.Owner == team)
            {
                continue;
            }

            float distance = Vector2.Distance(from, region.transform.position);
            if (distance < nearestDistance)
            {
                nearest = region;
                nearestDistance = distance;
            }
        }

        return nearest;
    }
}
