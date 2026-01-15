using UnityEngine;

[CreateAssetMenu(fileName = "NewTileData", menuName = "Kingmaker/Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string tileName;
    [SerializeField] private TileType tileType;
    [SerializeField, TextArea] private string description;

    [Header("Visuals")]
    [SerializeField] private Color tileColor = Color.white;

    [Header("Gameplay")]
    [SerializeField] private bool isPassable = true;
    [SerializeField] private int movementCost = 1;

    public string TileName => tileName;
    public TileType TileType => tileType;
    public string Description => description;
    public Color TileColor => tileColor;
    public bool IsPassable => isPassable;
    public int MovementCost => movementCost;
    public bool IsCapital => tileType == TileType.Capital;
}
