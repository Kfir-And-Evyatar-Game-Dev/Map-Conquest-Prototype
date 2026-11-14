using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Decides when the match ends: a team that holds enough regions for long enough wins;
/// when time runs out, the team with more regions wins (a tie is a draw).
/// </summary>
public class MatchRules : MonoBehaviour
{
    [SerializeField] private MatchSettings settings;

    private Region[] regions;
    private Team holdingTeam = Team.Neutral;
    private float holdSeconds;
    private float elapsedSeconds;
    private bool isOver;

    /// <summary>Fired every frame while the match runs: (seconds left, team holding enough regions, seconds that team still needs).</summary>
    public event Action<float, Team, float> StatusChanged;

    /// <summary>Fired once with the winner; Team.Neutral means a draw.</summary>
    public event Action<Team> GameEnded;

    private void Awake()
    {
        regions = FindObjectsByType<Region>(FindObjectsSortMode.None);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (isOver)
        {
            return;
        }

        elapsedSeconds += Time.deltaTime;
        UpdateHold();

        float secondsLeft = Mathf.Max(0f, settings.MatchSeconds - elapsedSeconds);
        float holdSecondsLeft = Mathf.Max(0f, settings.HoldSecondsToWin - holdSeconds);
        StatusChanged?.Invoke(secondsLeft, holdingTeam, holdSecondsLeft);

        if (holdingTeam != Team.Neutral && holdSecondsLeft <= 0f)
        {
            EndMatch(holdingTeam);
        }
        else if (secondsLeft <= 0f)
        {
            EndMatch(TeamWithMoreRegions());
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateHold()
    {
        Team leader = Team.Neutral;
        if (CountRegions(Team.Blue) >= settings.RegionsToWin)
        {
            leader = Team.Blue;
        }
        else if (CountRegions(Team.Red) >= settings.RegionsToWin)
        {
            leader = Team.Red;
        }

        if (leader != holdingTeam)
        {
            holdingTeam = leader;
            holdSeconds = 0f;
        }

        if (holdingTeam != Team.Neutral)
        {
            holdSeconds += Time.deltaTime;
        }
    }

    private Team TeamWithMoreRegions()
    {
        int blue = CountRegions(Team.Blue);
        int red = CountRegions(Team.Red);
        if (blue == red)
        {
            return Team.Neutral;
        }

        return blue > red ? Team.Blue : Team.Red;
    }

    // Bases don't count toward control.
    private int CountRegions(Team team)
    {
        int count = 0;
        foreach (Region region in regions)
        {
            if (!region.IsBase && region.Owner == team)
            {
                count++;
            }
        }

        return count;
    }

    private void EndMatch(Team winner)
    {
        isOver = true;
        Time.timeScale = 0f;
        GameEnded?.Invoke(winner);
    }
}
