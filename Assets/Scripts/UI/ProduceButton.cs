using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>A button that orders one unit type; greyed out while the team can't afford it.</summary>
[RequireComponent(typeof(Button))]
public class ProduceButton : MonoBehaviour
{
    [SerializeField] private UnitFactory factory;
    [SerializeField] private ResourceBank bank;
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private TMP_Text label;
    [SerializeField] private string unitName = "Infantry";

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        label.text = $"{unitName} ({unitPrefab.Stats.Cost})";
    }

    private void OnEnable()
    {
        button.onClick.AddListener(Produce);
        bank.AmountChanged += UpdateInteractable;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(Produce);
        bank.AmountChanged -= UpdateInteractable;
    }

    private void Start()
    {
        UpdateInteractable(bank.Amount);
    }

    private void Produce()
    {
        factory.TryProduce(unitPrefab);
    }

    private void UpdateInteractable(int amount)
    {
        button.interactable = amount >= unitPrefab.Stats.Cost;
    }
}