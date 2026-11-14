using TMPro;
using UnityEngine;

/// <summary>Shows a team's resources, updated through the bank's event.</summary>
public class ResourceLabel : MonoBehaviour
{
    [SerializeField] private ResourceBank bank;
    [SerializeField] private TMP_Text label;

    private void OnEnable()
    {
        bank.AmountChanged += Show;
    }

    private void OnDisable()
    {
        bank.AmountChanged -= Show;
    }

    private void Start()
    {
        Show(bank.Amount);
    }

    private void Show(int amount)
    {
        label.text = $"Resources: {amount}";
    }
}