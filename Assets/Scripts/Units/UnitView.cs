using UnityEngine;

/// <summary>Colors the unit by team and shows a ring while it is selected.</summary>
[RequireComponent(typeof(Unit), typeof(SpriteRenderer))]
public class UnitView : MonoBehaviour
{
    [SerializeField] private Color blueColor = new Color(0.35f, 0.55f, 1f);
    [SerializeField] private Color redColor = new Color(1f, 0.35f, 0.35f);
    [SerializeField] private GameObject selectionRing;

    private Unit unit;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        unit.SelectionChanged += ShowSelection;
    }

    private void OnDisable()
    {
        unit.SelectionChanged -= ShowSelection;
    }

    private void Start()
    {
        spriteRenderer.color = unit.Team == Team.Blue ? blueColor : redColor;
        ShowSelection(unit.IsSelected);
    }

    private void ShowSelection(bool isSelected)
    {
        selectionRing.SetActive(isSelected);
    }
}