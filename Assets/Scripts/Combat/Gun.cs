using UnityEngine;

/// <summary>Fires a shell at the nearest enemy in range, every few seconds.</summary>
[RequireComponent(typeof(Unit), typeof(Collider2D))]
public class Gun : MonoBehaviour
{
    [SerializeField] private Shell shellPrefab;
    [SerializeField, Min(0.1f)] private float range = 4f;
    [SerializeField, Min(0.1f)] private float secondsBetweenShots = 1.5f;
    [SerializeField, Min(0f)] private float launchForce = 8f;
    [SerializeField, Min(0f)] private float muzzleDistance = 0.7f;
    [SerializeField] private LayerMask targetLayers;

    private Unit unit;
    private Collider2D ownCollider;
    private float cooldown;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        ownCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown > 0f)
        {
            return;
        }

        Health target = FindNearestEnemy();
        if (target == null)
        {
            return;
        }

        Fire(target.transform.position);
        cooldown = secondsBetweenShots;
    }

    private Health FindNearestEnemy()
    {
        Health nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, range, targetLayers))
        {
            if (!hit.TryGetComponent(out Health health) || health.Team == unit.Team)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < nearestDistance)
            {
                nearest = health;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    private void Fire(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 muzzle = (Vector2)transform.position + direction * muzzleDistance;

        Shell shell = Instantiate(shellPrefab, muzzle, Quaternion.identity);
        shell.Launch(unit.Team, direction * launchForce, ownCollider);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
