using UnityEngine;

/// <summary>
/// Represents a flat-top hex tile in the 3D strategy game.
/// Uses axial coordinates (q, r) with flat-top orientation.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class FlatTopHexTile : MonoBehaviour
{
    [Header("Coordinates")]
    [SerializeField] private Vector2Int axial;
    
    [Header("Territory")]
    [SerializeField] private bool isOwned = false;
    [SerializeField] private int territoryId = -1;
    
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    
    public Vector2Int Axial => axial;
    public int Q => axial.x;
    public int R => axial.y;
    public bool IsOwned => isOwned;
    public int TerritoryId => territoryId;
    
    private void Awake()
    {
        CacheComponents();
    }
    
    private void CacheComponents()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        if (meshCollider == null)
            meshCollider = GetComponent<MeshCollider>();
    }
    
    /// <summary>
    /// Initialize the hex tile with axial coordinates.
    /// </summary>
    public void Initialize(int q, int r)
    {
        axial = new Vector2Int(q, r);
        gameObject.name = $"HexTile_{q}_{r}";
    }
    
    /// <summary>
    /// Initialize the hex tile with axial coordinates and a mesh.
    /// </summary>
    public void Initialize(int q, int r, Mesh hexMesh)
    {
        Initialize(q, r);
        CacheComponents();
        
        if (meshFilter != null && hexMesh != null)
        {
            meshFilter.sharedMesh = hexMesh;
        }
        
        if (meshCollider != null && hexMesh != null)
        {
            meshCollider.sharedMesh = hexMesh;
        }
    }
    
    /// <summary>
    /// Assign this tile to a territory.
    /// </summary>
    public void SetTerritory(int newTerritoryId, bool owned = true)
    {
        territoryId = newTerritoryId;
        isOwned = owned;
    }
    
    /// <summary>
    /// Clear territory ownership.
    /// </summary>
    public void ClearTerritory()
    {
        territoryId = -1;
        isOwned = false;
    }
    
    /// <summary>
    /// Set the material on this hex tile.
    /// </summary>
    public void SetMaterial(Material material)
    {
        CacheComponents();
        if (meshRenderer != null && material != null)
        {
            meshRenderer.sharedMaterial = material;
        }
    }
    
    /// <summary>
    /// Set the color of the hex tile (creates material instance).
    /// </summary>
    public void SetColor(Color color)
    {
        CacheComponents();
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }
    }
    
    /// <summary>
    /// Get HexCoordinates struct for use with existing coordinate math.
    /// </summary>
    public HexCoordinates GetHexCoordinates()
    {
        return new HexCoordinates(Q, R);
    }
}
