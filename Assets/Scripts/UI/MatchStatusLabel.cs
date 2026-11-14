using TMPro;
using UnityEngine;

/// <summary>Shows the match clock and who is close to winning, from MatchRules' status event.</summary>
public class MatchStatusLabel : MonoBehaviour
{
    [SerializeField] private MatchRules rules;
    [SerializeField] private TMP_Text label;

    private void OnEnable()
    {
        rules.StatusChanged += Show;
    }

    private void OnDisable()
    {
        rules.StatusChanged -= Show;
    }

    private void Show(float secondsLeft, Team holdingTeam, float holdSecondsLeft)
    {
        int minutes = Mathf.FloorToInt(secondsLeft / 60f);
        int seconds = Mathf.FloorToInt(secondsLeft % 60f);
        string clock = $"{minutes}:{seconds:00}";

        label.text = holdingTeam == Team.Neutral
            ? clock
            : $"{clock}   {holdingTeam} wins in {Mathf.CeilToInt(holdSecondsLeft)}s";
    }
}
