using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexGridManager : MonoBehaviour
{
    [SerializeField] private HexGridConfig config;
    [SerializeField] private Transform gridContainer;

    private Dictionary<HexCoordinates, HexTile> tiles = new Dictionary<HexCoordinates, HexTile>();

    public HexGridConfig Config => config;
    public IReadOnlyDictionary<HexCoordinates, HexTile> Tiles => tiles;

    [HideInInspector] public UnityEvent<HexTile> OnTileClicked = new UnityEvent<HexTile>();
    [HideInInspector] public UnityEvent<HexTile> OnTileHovered = new UnityEvent<HexTile>();
    [HideInInspector] public UnityEvent<HexTile> OnTileExited = new UnityEvent<HexTile>();

    public void GenerateGrid()
    {
        ClearGrid();

        for (int ring = 0; ring <= config.Radius; ring++)
        {
            if (ring == 0)
            {
                CreateHex(HexCoordinates.Zero, config.CapitalTileData);
            }
            else
            {
                foreach (var coords in GetRingCoordinates(ring))
                {
                    TileData data = config.GetTileDataForRing(ring);
                    CreateHex(coords, data);
                }
            }
        }

        Debug.Log($"Generated hex grid with {tiles.Count} tiles (radius {config.Radius})");
    }

    private IEnumerable<HexCoordinates> GetRingCoordinates(int ring)
    {
        if (ring <= 0) yield break;

        var current = new HexCoordinates(ring, 0);

        for (int side = 0; side < 6; side++)
        {
            for (int step = 0; step < ring; step++)
            {
                yield return current;
                current = current + HexCoordinates.Directions[(side + 2) % 6];
            }
        }
    }

    private void CreateHex(HexCoordinates coords, TileData data)
    {
        if (tiles.ContainsKey(coords)) return;
        if (config.HexTilePrefab == null)
        {
            Debug.LogError("HexTilePrefab is not assigned in HexGridConfig!");
            return;
        }

        Vector3 worldPos = config.HexToWorldPosition(coords);
        GameObject hexObj = Instantiate(config.HexTilePrefab, worldPos, Quaternion.identity, gridContainer);

        HexTile tile = hexObj.GetComponent<HexTile>();
        if (tile == null)
        {
            Debug.LogError("HexTilePrefab does not have HexTile component!");
            Destroy(hexObj);
            return;
        }

        tile.Initialize(coords, data);
        tile.OnTileClicked.AddListener(HandleTileClicked);
        tile.OnTileHovered.AddListener(HandleTileHovered);
        tile.OnTileExited.AddListener(HandleTileExited);

        tiles[coords] = tile;
    }

    public void ClearGrid()
    {
        foreach (var tile in tiles.Values)
        {
            if (tile != null)
            {
                tile.OnTileClicked.RemoveListener(HandleTileClicked);
                tile.OnTileHovered.RemoveListener(HandleTileHovered);
                tile.OnTileExited.RemoveListener(HandleTileExited);
                Destroy(tile.gameObject);
            }
        }
        tiles.Clear();
    }

    public HexTile GetTile(HexCoordinates coords)
    {
        tiles.TryGetValue(coords, out HexTile tile);
        return tile;
    }

    public List<HexTile> GetAdjacentTiles(HexCoordinates coords)
    {
        var adjacent = new List<HexTile>();
        foreach (var neighborCoords in coords.GetAllNeighbors())
        {
            if (tiles.TryGetValue(neighborCoords, out HexTile tile))
                adjacent.Add(tile);
        }
        return adjacent;
    }

    public bool AreAdjacent(HexCoordinates a, HexCoordinates b)
    {
        return a.DistanceTo(b) == 1;
    }

    public void HighlightTiles(IEnumerable<HexCoordinates> coordsList, Color color)
    {
        foreach (var coords in coordsList)
        {
            if (tiles.TryGetValue(coords, out HexTile tile))
                tile.SetHighlight(true, color);
        }
    }

    public void ClearAllHighlights()
    {
        foreach (var tile in tiles.Values)
            tile.SetHighlight(false);
    }

    private void HandleTileClicked(HexTile tile) => OnTileClicked?.Invoke(tile);
    private void HandleTileHovered(HexTile tile) => OnTileHovered?.Invoke(tile);
    private void HandleTileExited(HexTile tile) => OnTileExited?.Invoke(tile);

    private void OnDestroy()
    {
        ClearGrid();
    }
}
