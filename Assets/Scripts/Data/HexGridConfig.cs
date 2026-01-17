using UnityEngine;

[CreateAssetMenu(fileName = "HexGridConfig", menuName = "Kingmaker/Hex Grid Config")]
public class HexGridConfig : ScriptableObject
{
    [Header("Grid Dimensions")]
    [SerializeField, Range(1, 10)] private int radius = 2;

    [Header("Hex Geometry (Pointy-Top)")]
    [SerializeField] private float hexSize = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject hexTilePrefab;

    [Header("Default Tile Data")]
    [SerializeField] private TileData capitalTileData;
    [SerializeField] private TileData defaultTileData;

    [Header("Territory Tile Data (6 Directions)")]
    [SerializeField] private TileData plainsNorthData;      // Direction 0: East -> North equivalent
    [SerializeField] private TileData forestEastData;       // Direction 1: Northeast
    [SerializeField] private TileData villageNEData;        // Direction 2: Northwest -> NE territory
    [SerializeField] private TileData riverWestData;        // Direction 3: West
    [SerializeField] private TileData mountainSouthData;    // Direction 4: Southwest -> South
    [SerializeField] private TileData wildernessSEData;     // Direction 5: Southeast

    public int Radius => radius;
    public float HexSize => hexSize;
    public float HexWidth => hexSize * 2f;
    public float HexHeight => hexSize * Mathf.Sqrt(3f);
    public GameObject HexTilePrefab => hexTilePrefab;
    public TileData CapitalTileData => capitalTileData;
    public TileData DefaultTileData => defaultTileData;

    public TileData GetTileDataForRing(int ring)
    {
        if (ring == 0) return capitalTileData;
        return defaultTileData;
    }

    public TileData GetTileDataForCoordinates(HexCoordinates coords)
    {
        if (coords == HexCoordinates.Zero) return capitalTileData;

        int direction = GetDirectionFromCenter(coords);
        return GetTileDataForDirection(direction);
    }

    private int GetDirectionFromCenter(HexCoordinates coords)
    {
        // Convert hex coordinates to an angle and determine direction
        float angle = Mathf.Atan2(coords.R * 1.5f, coords.Q * Mathf.Sqrt(3f) + coords.R * Mathf.Sqrt(3f) / 2f);
        float degrees = angle * Mathf.Rad2Deg;

        // Normalize to 0-360
        if (degrees < 0) degrees += 360f;

        // Map to 6 directions (each 60 degrees)
        int direction = Mathf.FloorToInt((degrees + 30f) / 60f) % 6;
        return direction;
    }

    private TileData GetTileDataForDirection(int direction)
    {
        switch (direction)
        {
            case 0: return plainsNorthData ?? defaultTileData;
            case 1: return forestEastData ?? defaultTileData;
            case 2: return villageNEData ?? defaultTileData;
            case 3: return riverWestData ?? defaultTileData;
            case 4: return mountainSouthData ?? defaultTileData;
            case 5: return wildernessSEData ?? defaultTileData;
            default: return defaultTileData;
        }
    }

    public Vector3 HexToWorldPosition(HexCoordinates coords)
    {
        float x = hexSize * (Mathf.Sqrt(3f) * coords.Q + Mathf.Sqrt(3f) / 2f * coords.R);
        float y = hexSize * (3f / 2f * coords.R);
        return new Vector3(x, y, 0f);
    }

    public HexCoordinates WorldToHexCoordinates(Vector3 worldPos)
    {
        float q = (Mathf.Sqrt(3f) / 3f * worldPos.x - 1f / 3f * worldPos.y) / hexSize;
        float r = (2f / 3f * worldPos.y) / hexSize;
        return HexRound(q, r);
    }

    private HexCoordinates HexRound(float q, float r)
    {
        float s = -q - r;
        int roundQ = Mathf.RoundToInt(q);
        int roundR = Mathf.RoundToInt(r);
        int roundS = Mathf.RoundToInt(s);

        float qDiff = Mathf.Abs(roundQ - q);
        float rDiff = Mathf.Abs(roundR - r);
        float sDiff = Mathf.Abs(roundS - s);

        if (qDiff > rDiff && qDiff > sDiff)
            roundQ = -roundR - roundS;
        else if (rDiff > sDiff)
            roundR = -roundQ - roundS;

        return new HexCoordinates(roundQ, roundR);
    }
}
