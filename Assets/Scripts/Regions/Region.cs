using System;
using UnityEngine;

/// <summary>One area on the map: who owns it and which areas touch it.</summary>
public class Region : MonoBehaviour
{
    [SerializeField] private Team startingOwner = Team.Neutral;
    [SerializeField] private bool isBase;        // bases can't be captured
    [SerializeField] private bool isResource;    // ★ gives more income
    [SerializeField] private Region[] neighbors; // dragged in the Inspector

    public Team Owner { get; private set; }
    public bool IsBase => isBase;
    public bool IsResource => isResource;
    public Region[] Neighbors => neighbors;

    /// <summary>Fired when the owner changes: (this region, the new owner).</summary>
    public event Action<Region, Team> OwnerChanged;

    private void Awake()
    {
        Owner = startingOwner;
    }

    public void SetOwner(Team newOwner)
    {
        if (isBase || newOwner == Owner) { return; }

        Owner = newOwner;
        OwnerChanged?.Invoke(this, newOwner);
    }
}
