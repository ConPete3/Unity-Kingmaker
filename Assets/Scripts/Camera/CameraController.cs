using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 10f;
    [SerializeField] private bool useEdgePanning = false;
    [SerializeField] private float edgePanThreshold = 20f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;

    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-10, -10);
    [SerializeField] private Vector2 maxBounds = new Vector2(10, 10);

    private Camera cam;
    private Vector3 targetPosition;
    private bool isFollowing = true;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Start()
    {
        if (target != null)
        {
            targetPosition = target.position + offset;
            transform.position = targetPosition;
        }
    }

    private void Update()
    {
        HandleZoom();
        HandlePanning();

        if (isFollowing && target != null)
        {
            targetPosition = target.position + offset;
        }
    }

    private void LateUpdate()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        if (useBoundaries)
        {
            newPosition = ClampToBoundaries(newPosition);
        }

        transform.position = newPosition;
    }

    private void HandleZoom()
    {
        if (Mouse.current == null) return;

        float scrollDelta = Mouse.current.scroll.ReadValue().y / 120f; // Normalize scroll value
        if (Mathf.Abs(scrollDelta) > 0.01f && cam != null && cam.orthographic)
        {
            float newSize = cam.orthographicSize - scrollDelta * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    private void HandlePanning()
    {
        Vector3 panDelta = Vector3.zero;

        // WASD / Arrow key panning
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                panDelta.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                panDelta.y -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                panDelta.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                panDelta.x += 1;
        }

        // Edge-of-screen panning
        if (useEdgePanning && Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (mousePos.x < edgePanThreshold)
                panDelta.x -= 1;
            if (mousePos.x > Screen.width - edgePanThreshold)
                panDelta.x += 1;
            if (mousePos.y < edgePanThreshold)
                panDelta.y -= 1;
            if (mousePos.y > Screen.height - edgePanThreshold)
                panDelta.y += 1;
        }

        if (panDelta != Vector3.zero)
        {
            isFollowing = false;
            targetPosition += panDelta.normalized * panSpeed * Time.deltaTime;
        }
    }

    private Vector3 ClampToBoundaries(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        return position;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isFollowing = true;
    }

    public void SnapToTarget()
    {
        if (target != null)
        {
            targetPosition = target.position + offset;
            transform.position = targetPosition;
            isFollowing = true;
        }
    }

    public void ResumeFollowing()
    {
        isFollowing = true;
    }

    public void SetBoundaries(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBoundaries = true;
    }

    public void SetBoundariesFromGridSize(float gridRadius, float hexSize)
    {
        float extent = gridRadius * hexSize * 2f;
        minBounds = new Vector2(-extent, -extent);
        maxBounds = new Vector2(extent, extent);
        useBoundaries = true;
    }
}
