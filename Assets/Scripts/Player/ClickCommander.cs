using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Turns mouse clicks into orders: left click selects a unit, right click sends it to a region.</summary>
public class ClickCommander : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Team playerTeam = Team.Blue;
    [SerializeField] private LayerMask unitLayers;
    [SerializeField] private LayerMask regionLayers;

    private Unit selected;

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Select(FindUnderMouse<Unit>(mouse, unitLayers));
        }

        if (mouse.rightButton.wasPressedThisFrame && selected != null)
        {
            Region target = FindUnderMouse<Region>(mouse, regionLayers);
            if (target != null)
            {
                selected.GetComponent<UnitMover>().MoveTo(target.transform.position);
            }
        }
    }

    // Physics2D.Raycast at the mouse point: returns what lies under the cursor on the given layers.
    private T FindUnderMouse<T>(Mouse mouse, LayerMask layers) where T : Component
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, layers);
        return hit.collider != null ? hit.collider.GetComponent<T>() : null;
    }

    private void Select(Unit unit)
    {
        if (selected != null)
        {
            selected.SetSelected(false);
        }

        selected = unit != null && unit.Team == playerTeam ? unit : null;

        if (selected != null)
        {
            selected.SetSelected(true);
        }
    }
}