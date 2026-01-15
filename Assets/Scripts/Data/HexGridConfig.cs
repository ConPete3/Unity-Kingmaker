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
