using System;
using UnityEngine;

/// <summary>A unit on the map: its team, its stats and whether the player selected it.</summary>
public class Unit : MonoBehaviour
{
    [SerializeField] private UnitStats stats;
    [SerializeField] private Team team = Team.Blue;

    public UnitStats Stats => stats;
    public Team Team => team;
    public bool IsSelected { get; private set; }

    /// <summary>Fired when the unit is selected (true) or deselected (false).</summary>
    public event Action<bool> SelectionChanged;

    public void SetSelected(bool isSelected)
    {
        if (isSelected == IsSelected)
        {
            return;
        }

        IsSelected = isSelected;
        SelectionChanged?.Invoke(isSelected);
    }
}