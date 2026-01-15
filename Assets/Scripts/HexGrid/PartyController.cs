using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PartyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HexGridManager hexGridManager;
    [SerializeField] private Transform partyToken;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private HexCoordinates currentPosition;
    private bool isMoving;

    public HexCoordinates CurrentPosition => currentPosition;
    public bool IsMoving => isMoving;

    [HideInInspector] public UnityEvent OnPartyMoved = new UnityEvent();
    [HideInInspector] public UnityEvent<HexCoordinates> OnMovementStarted = new UnityEvent<HexCoordinates>();
    [HideInInspector] public UnityEvent<HexCoordinates> OnMovementCompleted = new UnityEvent<HexCoordinates>();

    private void Start()
    {
        currentPosition = HexCoordinates.Zero;
        SnapToCurrentPosition();
    }

    public bool CanMoveTo(HexCoordinates targetCoords)
    {
        if (isMoving) return false;

        if (!hexGridManager.AreAdjacent(currentPosition, targetCoords))
            return false;

        HexTile targetTile = hexGridManager.GetTile(targetCoords);
        if (targetTile == null || targetTile.TileData == null)
            return false;

        return targetTile.TileData.IsPassable;
    }

    public void MoveTo(HexCoordinates targetCoords)
    {
        if (!CanMoveTo(targetCoords)) return;

        StartCoroutine(MoveCoroutine(targetCoords));
    }

    private IEnumerator MoveCoroutine(HexCoordinates targetCoords)
    {
        isMoving = true;
        OnMovementStarted?.Invoke(targetCoords);

        Vector3 startPos = partyToken.position;
        Vector3 endPos = hexGridManager.Config.HexToWorldPosition(targetCoords);

        float elapsed = 0f;
        float duration = Vector3.Distance(startPos, endPos) / moveSpeed;

        if (duration < 0.01f) duration = 0.1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            partyToken.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        partyToken.position = endPos;
        currentPosition = targetCoords;
        isMoving = false;

        OnMovementCompleted?.Invoke(targetCoords);
        OnPartyMoved?.Invoke();
    }

    public void SnapToCurrentPosition()
    {
        if (hexGridManager != null && hexGridManager.Config != null && partyToken != null)
        {
            Vector3 worldPos = hexGridManager.Config.HexToWorldPosition(currentPosition);
            partyToken.position = worldPos;
        }
    }

    public bool IsPartyOnTile(HexCoordinates coords)
    {
        return currentPosition == coords && !isMoving;
    }

    public List<HexTile> GetMoveableTiles()
    {
        var moveable = new List<HexTile>();
        var neighbors = currentPosition.GetAllNeighbors();

        foreach (var neighborCoords in neighbors)
        {
            HexTile tile = hexGridManager.GetTile(neighborCoords);
            if (tile != null && tile.TileData != null && tile.TileData.IsPassable)
            {
                moveable.Add(tile);
            }
        }

        return moveable;
    }

    public void SetPosition(HexCoordinates coords)
    {
        currentPosition = coords;
        SnapToCurrentPosition();
        OnPartyMoved?.Invoke();
    }
}
