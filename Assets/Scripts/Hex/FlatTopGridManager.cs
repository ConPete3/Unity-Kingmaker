using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages generation of flat-top hex grids using axial coordinates.
/// Flat-top formula: x = radius * (3/2 * q), z = radius * (sqrt(3)/2 * q + sqrt(3) * r)
/// </summary>
public class FlatTopGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private float hexRadius = 1f;
    
    [Header("References")]
    [SerializeField] private GameObject hexTilePrefab;
    [SerializeField] private Material defaultMaterial;
    
    [Header("Generated")]
    [SerializeField] private Transform gridRoot;
    
    private Dictionary<Vector2Int, FlatTopHexTile> tileMap = new Dictionary<Vector2Int, FlatTopHexTile>();
    private Mesh sharedHexMesh;
    
    // Flat-top hex constants
    private static readonly float SQRT3 = Mathf.Sqrt(3f);
    
    [Header("Auto Generation")]
    [SerializeField] private bool autoGenerateOnStart = true;

    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;
    public float HexRadius => hexRadius;
    public IReadOnlyDictionary<Vector2Int, FlatTopHexTile> Tiles => tileMap;

    private void Start()
    {
        if (autoGenerateOnStart && tileMap.Count == 0 && gridRoot == null)
        {
            GenerateGrid();
        }
    }

    /// <summary>
    /// Convert axial coordinates to world position (flat-top orientation).
    /// </summary>
    public Vector3 AxialToWorld(int q, int r)
    {
        float x = hexRadius * (1.5f * q);
        float z = hexRadius * (SQRT3 * 0.5f * q + SQRT3 * r);
        return new Vector3(x, 0f, z);
    }
    
    /// <summary>
    /// Convert world position to axial coordinates (flat-top orientation).
    /// </summary>
    public Vector2Int WorldToAxial(Vector3 worldPos)
    {
        float q = (2f / 3f * worldPos.x) / hexRadius;
        float r = (-1f / 3f * worldPos.x + SQRT3 / 3f * worldPos.z) / hexRadius;
        return AxialRound(q, r);
    }
    
    private Vector2Int AxialRound(float q, float r)
    {
        float s = -q - r;
        int qi = Mathf.RoundToInt(q);
        int ri = Mathf.RoundToInt(r);
        int si = Mathf.RoundToInt(s);
        
        float qDiff = Mathf.Abs(qi - q);
        float rDiff = Mathf.Abs(ri - r);
        float sDiff = Mathf.Abs(si - s);
        
        if (qDiff > rDiff && qDiff > sDiff)
            qi = -ri - si;
        else if (rDiff > sDiff)
            ri = -qi - si;
        
        return new Vector2Int(qi, ri);
    }
    
    /// <summary>
    /// Get a tile at the specified axial coordinates.
    /// </summary>
    public FlatTopHexTile GetTile(int q, int r)
    {
        var key = new Vector2Int(q, r);
        return tileMap.TryGetValue(key, out var tile) ? tile : null;
    }
    
    /// <summary>
    /// Get the bounds of the generated grid.
    /// </summary>
    public Bounds GetGridBounds()
    {
        if (tileMap.Count == 0)
            return new Bounds(Vector3.zero, Vector3.one);
            
        Vector3 min = Vector3.one * float.MaxValue;
        Vector3 max = Vector3.one * float.MinValue;
        
        foreach (var tile in tileMap.Values)
        {
            Vector3 pos = tile.transform.position;
            min = Vector3.Min(min, pos);
            max = Vector3.Max(max, pos);
        }
        
        // Add hex radius padding
        min -= Vector3.one * hexRadius;
        max += Vector3.one * hexRadius;
        
        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;
        
        return new Bounds(center, size);
    }
    
    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        ClearGrid();
        CreateGridRoot();
        CreateSharedMesh();
        
        // Generate rectangular grid using offset coordinates converted to axial
        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                // Convert offset to axial (flat-top, odd-q offset)
                int q = col;
                int r = row - (col / 2);
                
                CreateHexTile(q, r);
            }
        }
        
        Debug.Log($"[FlatTopGridManager] Generated {tileMap.Count} hex tiles");
    }
    
    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        if (gridRoot != null)
        {
            if (Application.isPlaying)
                Destroy(gridRoot.gameObject);
            else
                DestroyImmediate(gridRoot.gameObject);
        }
        
        tileMap.Clear();
        gridRoot = null;
    }
    
    private void CreateGridRoot()
    {
        var rootGO = new GameObject("HexGridRoot");
        rootGO.transform.SetParent(transform);
        rootGO.transform.localPosition = Vector3.zero;
        rootGO.transform.localRotation = Quaternion.identity;
        gridRoot = rootGO.transform;
    }
    
    private void CreateHexTile(int q, int r)
    {
        Vector3 worldPos = AxialToWorld(q, r);
        
        GameObject tileGO;
        
        if (hexTilePrefab != null)
        {
            tileGO = Instantiate(hexTilePrefab, worldPos, Quaternion.identity, gridRoot);
        }
        else
        {
            // Create tile from scratch
            tileGO = new GameObject($"HexTile_{q}_{r}");
            tileGO.transform.SetParent(gridRoot);
            tileGO.transform.position = worldPos;
            
            // Add required components
            var meshFilter = tileGO.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = sharedHexMesh;
            
            var meshRenderer = tileGO.AddComponent<MeshRenderer>();
            if (defaultMaterial != null)
                meshRenderer.sharedMaterial = defaultMaterial;
            else
                meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            
            var meshCollider = tileGO.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = sharedHexMesh;
        }
        
        // Get or add FlatTopHexTile component
        var hexTile = tileGO.GetComponent<FlatTopHexTile>();
        if (hexTile == null)
            hexTile = tileGO.AddComponent<FlatTopHexTile>();
        
        hexTile.Initialize(q, r, sharedHexMesh);
        
        var key = new Vector2Int(q, r);
        tileMap[key] = hexTile;
    }
    
    /// <summary>
    /// Create a flat-top hexagon prism mesh with 3D thickness.
    /// </summary>
    private void CreateSharedMesh()
    {
        if (sharedHexMesh != null)
            return;

        sharedHexMesh = new Mesh();
        sharedHexMesh.name = "FlatTopHex3D";

        const float halfHeight = 0.125f; // Total height = 0.25

        // Flat-top hex: corners at angles 0°, 60°, 120°, 180°, 240°, 300°
        // We need separate vertices for top, bottom, and sides to have correct normals

        // Calculate corner positions
        Vector3[] cornerPositions = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = 60f * i * Mathf.Deg2Rad;
            cornerPositions[i] = new Vector3(
                hexRadius * Mathf.Cos(angle),
                0f,
                hexRadius * Mathf.Sin(angle)
            );
        }

        // Vertex layout:
        // 0: top center
        // 1-6: top corners
        // 7: bottom center
        // 8-13: bottom corners
        // 14-37: side faces (4 verts per side x 6 sides = 24 verts)

        Vector3[] vertices = new Vector3[38];
        Vector3[] normals = new Vector3[38];
        Vector2[] uvs = new Vector2[38];

        // Top face (7 vertices: center + 6 corners)
        vertices[0] = new Vector3(0f, halfHeight, 0f);
        normals[0] = Vector3.up;
        uvs[0] = new Vector2(0.5f, 0.5f);

        for (int i = 0; i < 6; i++)
        {
            vertices[i + 1] = new Vector3(cornerPositions[i].x, halfHeight, cornerPositions[i].z);
            normals[i + 1] = Vector3.up;
            float angle = 60f * i * Mathf.Deg2Rad;
            uvs[i + 1] = new Vector2(0.5f + 0.5f * Mathf.Cos(angle), 0.5f + 0.5f * Mathf.Sin(angle));
        }

        // Bottom face (7 vertices: center + 6 corners)
        vertices[7] = new Vector3(0f, -halfHeight, 0f);
        normals[7] = Vector3.down;
        uvs[7] = new Vector2(0.5f, 0.5f);

        for (int i = 0; i < 6; i++)
        {
            vertices[i + 8] = new Vector3(cornerPositions[i].x, -halfHeight, cornerPositions[i].z);
            normals[i + 8] = Vector3.down;
            float angle = 60f * i * Mathf.Deg2Rad;
            uvs[i + 8] = new Vector2(0.5f + 0.5f * Mathf.Cos(angle), 0.5f + 0.5f * Mathf.Sin(angle));
        }

        // Side faces (4 vertices per side, 6 sides)
        int sideVertIndex = 14;
        for (int i = 0; i < 6; i++)
        {
            int next = (i + 1) % 6;

            Vector3 topA = new Vector3(cornerPositions[i].x, halfHeight, cornerPositions[i].z);
            Vector3 topB = new Vector3(cornerPositions[next].x, halfHeight, cornerPositions[next].z);
            Vector3 botA = new Vector3(cornerPositions[i].x, -halfHeight, cornerPositions[i].z);
            Vector3 botB = new Vector3(cornerPositions[next].x, -halfHeight, cornerPositions[next].z);

            // Calculate outward normal for this side
            Vector3 edge = topB - topA;
            Vector3 sideNormal = Vector3.Cross(edge, Vector3.down).normalized;

            vertices[sideVertIndex] = topA;
            vertices[sideVertIndex + 1] = topB;
            vertices[sideVertIndex + 2] = botA;
            vertices[sideVertIndex + 3] = botB;

            normals[sideVertIndex] = sideNormal;
            normals[sideVertIndex + 1] = sideNormal;
            normals[sideVertIndex + 2] = sideNormal;
            normals[sideVertIndex + 3] = sideNormal;

            // UV: map each side face to a 0-1 quad
            uvs[sideVertIndex] = new Vector2(0f, 1f);
            uvs[sideVertIndex + 1] = new Vector2(1f, 1f);
            uvs[sideVertIndex + 2] = new Vector2(0f, 0f);
            uvs[sideVertIndex + 3] = new Vector2(1f, 0f);

            sideVertIndex += 4;
        }

        // Triangles
        // Top face: 6 triangles
        // Bottom face: 6 triangles
        // Side faces: 2 triangles per side x 6 sides = 12 triangles
        // Total: 24 triangles x 3 = 72 indices

        int[] triangles = new int[72];
        int triIndex = 0;

        // Top face triangles (CCW when viewed from above)
        for (int i = 0; i < 6; i++)
        {
            triangles[triIndex++] = 0;
            triangles[triIndex++] = i + 1;
            triangles[triIndex++] = (i < 5) ? i + 2 : 1;
        }

        // Bottom face triangles (CW when viewed from above = CCW from below)
        for (int i = 0; i < 6; i++)
        {
            triangles[triIndex++] = 7;
            triangles[triIndex++] = (i < 5) ? i + 9 : 8;
            triangles[triIndex++] = i + 8;
        }

        // Side face triangles
        sideVertIndex = 14;
        for (int i = 0; i < 6; i++)
        {
            // Two triangles per side quad
            // Quad vertices: topA, topB, botA, botB
            triangles[triIndex++] = sideVertIndex;     // topA
            triangles[triIndex++] = sideVertIndex + 2; // botA
            triangles[triIndex++] = sideVertIndex + 1; // topB

            triangles[triIndex++] = sideVertIndex + 1; // topB
            triangles[triIndex++] = sideVertIndex + 2; // botA
            triangles[triIndex++] = sideVertIndex + 3; // botB

            sideVertIndex += 4;
        }

        sharedHexMesh.vertices = vertices;
        sharedHexMesh.triangles = triangles;
        sharedHexMesh.uv = uvs;
        sharedHexMesh.normals = normals;
        sharedHexMesh.RecalculateBounds();
    }
}
