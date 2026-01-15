using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class HexTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private SpriteRenderer borderRenderer;

    private HexCoordinates coordinates;
    private TileData tileData;
    private bool isHighlighted;

    public HexCoordinates Coordinates => coordinates;
    public TileData TileData => tileData;
    public bool IsHighlighted => isHighlighted;

    [HideInInspector] public UnityEvent<HexTile> OnTileClicked = new UnityEvent<HexTile>();
    [HideInInspector] public UnityEvent<HexTile> OnTileHovered = new UnityEvent<HexTile>();
    [HideInInspector] public UnityEvent<HexTile> OnTileExited = new UnityEvent<HexTile>();

    public void Initialize(HexCoordinates coords, TileData data)
    {
        coordinates = coords;
        tileData = data;

        if (spriteRenderer != null && data != null)
        {
            spriteRenderer.color = data.TileColor;
        }

        gameObject.name = $"Hex_{coords.Q}_{coords.R}_{data?.TileName ?? "Empty"}";
    }

    public void SetHighlight(bool highlighted, Color? color = null)
    {
        isHighlighted = highlighted;
        if (highlightRenderer != null)
        {
            highlightRenderer.enabled = highlighted;
            if (color.HasValue)
                highlightRenderer.color = color.Value;
        }
    }

    public void SetBorderColor(Color color)
    {
        if (borderRenderer != null)
            borderRenderer.color = color;
    }

    private void OnMouseDown()
    {
        OnTileClicked?.Invoke(this);
    }

    private void OnMouseEnter()
    {
        OnTileHovered?.Invoke(this);
    }

    private void OnMouseExit()
    {
        OnTileExited?.Invoke(this);
    }
}
