using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a player's kingdom with resources and owned territories.
/// </summary>
public class PlayerKingdom : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private int playerId = 0;
    [SerializeField] private string kingdomName = "Player";
    [SerializeField] private Color kingdomColor = Color.blue;
    
    [Header("Resources (Placeholder)")]
    [SerializeField] private int gold = 100;
    [SerializeField] private int food = 50;
    [SerializeField] private int materials = 25;
    
    [Header("Territory")]
    [SerializeField] private List<FlatTopHexTile> ownedTiles = new List<FlatTopHexTile>();
    
    public int PlayerId => playerId;
    public string KingdomName => kingdomName;
    public Color KingdomColor => kingdomColor;
    public int Gold => gold;
    public int Food => food;
    public int Materials => materials;
    public IReadOnlyList<FlatTopHexTile> OwnedTiles => ownedTiles;
    public int TileCount => ownedTiles.Count;
    
    /// <summary>
    /// Initialize the kingdom with player settings.
    /// </summary>
    public void Initialize(int id, string name, Color color)
    {
        playerId = id;
        kingdomName = name;
        kingdomColor = color;
        gameObject.name = $"Kingdom_{name}";
    }
    
    /// <summary>
    /// Add a tile to this kingdom's territory.
    /// </summary>
    public void AddTile(FlatTopHexTile tile)
    {
        if (tile == null || ownedTiles.Contains(tile))
            return;
            
        ownedTiles.Add(tile);
        tile.SetTerritory(playerId, true);
    }
    
    /// <summary>
    /// Remove a tile from this kingdom's territory.
    /// </summary>
    public void RemoveTile(FlatTopHexTile tile)
    {
        if (tile == null || !ownedTiles.Contains(tile))
            return;
            
        ownedTiles.Remove(tile);
        tile.ClearTerritory();
    }
    
    /// <summary>
    /// Check if this kingdom owns a specific tile.
    /// </summary>
    public bool OwnsTile(FlatTopHexTile tile)
    {
        return tile != null && ownedTiles.Contains(tile);
    }
    
    /// <summary>
    /// Modify gold amount.
    /// </summary>
    public void ModifyGold(int amount)
    {
        gold = Mathf.Max(0, gold + amount);
    }
    
    /// <summary>
    /// Modify food amount.
    /// </summary>
    public void ModifyFood(int amount)
    {
        food = Mathf.Max(0, food + amount);
    }
    
    /// <summary>
    /// Modify materials amount.
    /// </summary>
    public void ModifyMaterials(int amount)
    {
        materials = Mathf.Max(0, materials + amount);
    }
    
    /// <summary>
    /// Set all resources at once.
    /// </summary>
    public void SetResources(int newGold, int newFood, int newMaterials)
    {
        gold = Mathf.Max(0, newGold);
        food = Mathf.Max(0, newFood);
        materials = Mathf.Max(0, newMaterials);
    }
}
