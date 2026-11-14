using System;
using UnityEngine;

/// <summary>Holds one team's resources and announces every change.</summary>
public class ResourceBank : MonoBehaviour
{
    [SerializeField] private Team team = Team.Blue;
    [SerializeField] private MatchSettings settings;

    public Team Team => team;
    public int Amount { get; private set; }

    /// <summary>Fired with the new amount after every change.</summary>
    public event Action<int> AmountChanged;

    private void Awake()
    {
        Amount = settings.StartingResources;
    }

    public void Add(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Amount += amount;
        AmountChanged?.Invoke(Amount);
    }

    public bool TrySpend(int amount)
    {
        if (amount > Amount)
        {
            return false;
        }

        Amount -= amount;
        AmountChanged?.Invoke(Amount);
        return true;
    }
}