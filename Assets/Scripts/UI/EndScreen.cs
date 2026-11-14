using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Shows Victory / Defeat / Draw when the match ends, with a Play Again button.</summary>
public class EndScreen : MonoBehaviour
{
    [SerializeField] private MatchRules rules;
    [SerializeField] private Team playerTeam = Team.Blue;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text title;
    [SerializeField] private Button playAgainButton;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        rules.GameEnded += Show;
        playAgainButton.onClick.AddListener(rules.Restart);
    }

    private void OnDisable()
    {
        rules.GameEnded -= Show;
        playAgainButton.onClick.RemoveListener(rules.Restart);
    }

    private void Show(Team winner)
    {
        if (winner == Team.Neutral)
        {
            title.text = "Draw";
        }
        else
        {
            title.text = winner == playerTeam ? "Victory!" : "Defeat";
        }

        panel.SetActive(true);
    }
}
