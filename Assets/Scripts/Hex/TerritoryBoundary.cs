using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Renders a territory boundary using LineRenderer.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class TerritoryBoundary : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private Color lineColor = Color.yellow;
    [SerializeField] private float heightOffset = 0.05f;
    
    private LineRenderer lineRenderer;
    private List<Vector3> boundaryPoints = new List<Vector3>();
    
    public int TerritoryId { get; private set; } = -1;
    
    private void Awake()
    {
        CacheComponents();
        SetupLineRenderer();
    }
    
    private void CacheComponents()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }
    
    private void SetupLineRenderer()
    {
        CacheComponents();
        if (lineRenderer != null)
        {
            lineRenderer.loop = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.useWorldSpace = true;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }
    }
    
    /// <summary>
    /// Initialize the boundary for a specific territory.
    /// </summary>
    public void Initialize(int territoryId)
    {
        TerritoryId = territoryId;
        gameObject.name = $"TerritoryBoundary_{territoryId}";
    }
    
    /// <summary>
    /// Set the boundary points to render.
    /// </summary>
    public void SetBoundary(List<Vector3> points)
    {
        CacheComponents();
        boundaryPoints.Clear();
        
        if (points == null || points.Count < 3)
        {
            if (lineRenderer != null)
                lineRenderer.positionCount = 0;
            return;
        }
        
        // Add height offset to all points
        foreach (var point in points)
        {
            boundaryPoints.Add(new Vector3(point.x, point.y + heightOffset, point.z));
        }
        
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = boundaryPoints.Count;
            lineRenderer.SetPositions(boundaryPoints.ToArray());
        }
    }
    
    /// <summary>
    /// Set boundary visibility.
    /// </summary>
    public void SetVisible(bool visible)
    {
        CacheComponents();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = visible;
        }
    }
    
    /// <summary>
    /// Set the boundary line color.
    /// </summary>
    public void SetColor(Color color)
    {
        lineColor = color;
        CacheComponents();
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
    
    /// <summary>
    /// Set the boundary line width.
    /// </summary>
    public void SetWidth(float width)
    {
        lineWidth = width;
        CacheComponents();
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }
    
    /// <summary>
    /// Clear the boundary.
    /// </summary>
    public void ClearBoundary()
    {
        boundaryPoints.Clear();
        CacheComponents();
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
