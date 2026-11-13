using UnityEngine;

/// <summary>Paints a region's square in its owner's color, faded when cut off from its base.</summary>
[RequireComponent(typeof(Region), typeof(SpriteRenderer))]
public class RegionView : MonoBehaviour
{
    [SerializeField] private Color neutralColor = Color.gray;
    [SerializeField] private Color blueColor = new Color(0.2f, 0.4f, 0.9f);
    [SerializeField] private Color redColor = new Color(0.85f, 0.2f, 0.2f);
    [SerializeField, Range(0f, 1f)] private float disconnectedAlpha = 0.35f;

    private Region region;
    private SpriteRenderer spriteRenderer;
    private SupplyNetwork network;

    private void Awake()
    {
        region = GetComponent<Region>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        network = FindFirstObjectByType<SupplyNetwork>();
    }

    private void OnEnable()
    {
        region.OwnerChanged += HandleOwnerChanged;
        if (network != null)
        {
            network.NetworkChanged += Repaint;
        }
    }

    private void OnDisable()
    {
        region.OwnerChanged -= HandleOwnerChanged;
        if (network != null)
        {
            network.NetworkChanged -= Repaint;
        }
    }

    private void Start()
    {
        Repaint();
    }

    private void HandleOwnerChanged(Region changedRegion, Team newOwner)
    {
        Repaint();
    }

    private void Repaint()
    {
        Color color = region.Owner switch
        {
            Team.Blue => blueColor,
            Team.Red => redColor,
            _ => neutralColor,
        };

        bool isCutOff = region.Owner != Team.Neutral && network != null && !network.IsConnected(region);
        color.a = isCutOff ? disconnectedAlpha : 1f;
        spriteRenderer.color = color;
    }
}