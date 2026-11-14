using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Captures the region for a team whose capturing units stand in it alone long enough.</summary>
[RequireComponent(typeof(Region))]
public class RegionCapture : MonoBehaviour
{
    private readonly List<Unit> unitsInside = new List<Unit>();
    private Region region;
    private float progressSeconds;

    /// <summary>Capture progress from 0 to 1, for a progress bar.</summary>
    public event Action<float> ProgressChanged;

    private void Awake()
    {
        region = GetComponent<Region>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Unit unit) && !unitsInside.Contains(unit))
        {
            unitsInside.Add(unit);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Unit unit))
        {
            unitsInside.Remove(unit);
        }
    }

    private void Update()
    {
        unitsInside.RemoveAll(unit => unit == null); // a destroyed unit never "exits"

        Unit capturer = FindCapturer();
        if (capturer == null)
        {
            ResetProgress();
            return;
        }

        float neededSeconds = capturer.Stats.CaptureSeconds;
        progressSeconds += Time.deltaTime;
        ProgressChanged?.Invoke(Mathf.Clamp01(progressSeconds / neededSeconds));

        if (progressSeconds >= neededSeconds)
        {
            region.SetOwner(capturer.Team);
            ResetProgress();
        }
    }

    // Returns a unit that can capture, or null when the region is empty, contested,
    // already owned by that team, a base, or holds only units that can't capture (tanks).
    private Unit FindCapturer()
    {
        if (region.IsBase || unitsInside.Count == 0)
        {
            return null;
        }

        Team team = unitsInside[0].Team;
        Unit capturer = null;
        foreach (Unit unit in unitsInside)
        {
            if (unit.Team != team)
            {
                return null; // both teams are inside: nobody captures
            }

            if (unit.Stats.CanCapture)
            {
                capturer = unit;
            }
        }

        return team != region.Owner ? capturer : null;
    }

    private void ResetProgress()
    {
        if (progressSeconds == 0f)
        {
            return;
        }

        progressSeconds = 0f;
        ProgressChanged?.Invoke(0f);
    }
}