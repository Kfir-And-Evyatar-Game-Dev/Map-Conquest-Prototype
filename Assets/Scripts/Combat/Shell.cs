using UnityEngine;

/// <summary>A projectile launched by a force; damages and pushes back the enemy it hits.</summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Shell : MonoBehaviour
{
    [SerializeField, Min(0)] private int damage = 40;
    [SerializeField, Min(0f)] private float knockbackForce = 3f;
    [SerializeField, Min(0.1f)] private float lifetimeSeconds = 2f;

    private Rigidbody2D body;
    private Team ownerTeam;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Launch(Team team, Vector2 force, Collider2D shooterCollider)
    {
        ownerTeam = team;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooterCollider);
        body.AddForce(force, ForceMode2D.Impulse);
        Destroy(gameObject, lifetimeSeconds);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check the properties of what we hit: can it take damage, and is it an enemy?
        if (collision.gameObject.TryGetComponent(out IDamageable target) && target.Team != ownerTeam)
        {
            target.TakeDamage(damage);

            if (collision.rigidbody != null)
            {
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                collision.rigidbody.AddForce(pushDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
