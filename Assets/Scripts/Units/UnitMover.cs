using UnityEngine;

/// <summary>Moves the unit in a straight line to a destination, through its Rigidbody2D.</summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Unit))]
public class UnitMover : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float arriveDistance = 0.1f;

    private Rigidbody2D body;
    private Unit unit;
    private Vector2? destination;

    public bool IsMoving => destination.HasValue;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        unit = GetComponent<Unit>();
    }

    public void MoveTo(Vector2 target)
    {
        destination = target;
    }

    public void Stop()
    {
        destination = null;
    }

    private void FixedUpdate()
    {
        if (!destination.HasValue)
        {
            return;
        }

        Vector2 toTarget = destination.Value - body.position;
        if (toTarget.magnitude <= arriveDistance)
        {
            destination = null;
            return;
        }

        float step = unit.Stats.MoveSpeed * Time.fixedDeltaTime;
        body.MovePosition(body.position + Vector2.ClampMagnitude(toTarget, step));
    }
}