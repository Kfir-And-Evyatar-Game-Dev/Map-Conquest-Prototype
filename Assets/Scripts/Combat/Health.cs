using System;
using UnityEngine;

/// <summary>A unit's hit points; destroys the unit when they run out.</summary>
[RequireComponent(typeof(Unit))]
public class Health : MonoBehaviour, IDamageable
{
    private Unit unit;

    public int Current { get; private set; }
    public Team Team => unit.Team;

    /// <summary>Fired after damage: (current, max).</summary>
    public event Action<int, int> Changed;

    /// <summary>Fired once, right before the unit is destroyed.</summary>
    public event Action<Health> Died;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        Current = unit.Stats.MaxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || Current <= 0)
        {
            return;
        }

        Current = Mathf.Max(0, Current - amount);
        Changed?.Invoke(Current, unit.Stats.MaxHealth);

        if (Current == 0)
        {
            Died?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
