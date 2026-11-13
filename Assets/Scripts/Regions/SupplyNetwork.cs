using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Knows which regions are linked to their team's base through friendly regions.</summary>
public class SupplyNetwork : MonoBehaviour
{
    private readonly HashSet<Region> connected = new HashSet<Region>();
    private Region[] regions;

    /// <summary>Fired after the network is recalculated.</summary>
    public event Action NetworkChanged;

    private void Awake()
    {
        regions = FindObjectsByType<Region>(FindObjectsSortMode.None);
    }

    private void OnEnable()
    {
        foreach (Region region in regions)
        {
            region.OwnerChanged += HandleOwnerChanges;
        }
    }

    private void OnDisable()
    {
        foreach (Region region in regions)
        {
            region.OwnerChanged -= HandleOwnerChanges;
        }
    }

    private void Start()
    {
        Recalculate();
    }

    public bool IsConnected(Region region)
    {
        return connected.Contains(region);
    }

    private void HandleOwnerChanges(Region region, Team newOwner)
    {
        Recalculate();
    }

    private void Recalculate()
    {
        connected.Clear();
        foreach (Region region in regions)
        {
            if (region.IsBase)
            {
                SpreadFrom(region);
            }
        }

        NetworkChanged?.Invoke();
    }

    // Breadth-first search: walk out from the base through neighbors of the same team.
    private void SpreadFrom(Region baseRegion)
    {
        var queue = new Queue<Region>();
        queue.Enqueue(baseRegion);
        connected.Add(baseRegion);

        while (queue.Count > 0)
        {
            Region current = queue.Dequeue();
            foreach (Region neighbor in current.Neighbors)
            {
                if (neighbor.Owner == baseRegion.Owner && connected.Add(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }
    }
}