using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Strategy camera controller with fixed 45° tilt perspective view.
/// Supports WASD pan, middle-mouse drag pan, and scroll zoom.
/// Uses new Input System.
/// </summary>
public class StrategyCameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float panSpeed = 10f;
    [SerializeField] private float dragPanSpeed = 0.5f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;
    [SerializeField] private float currentZoom = 15f;

    [Header("Camera Angle")]
    [SerializeField] private float tiltAngle = 45f;

    [Header("Bounds")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Bounds cameraBounds;

    private Camera cam;
    private Vector2 lastMousePosition;
    private bool isDragging;

    // Track target position for smooth movement
    private Vector3 targetPosition;

    // Input System references
    private Mouse mouse;
    private Keyboard keyboard;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;

        mouse = Mouse.current;
        keyboard = Keyboard.current;
    }

    private void Start()
    {
        InitializeCamera();
    }

    /// <summary>
    /// Initialize camera position and rotation.
    /// </summary>
    public void InitializeCamera()
    {
        if (cam != null)
        {
            cam.orthographic = false; // Perspective camera
        }

        // Set initial rotation
        transform.rotation = Quaternion.Euler(tiltAngle, 0f, 0f);

        // Set initial position based on zoom
        targetPosition = CalculatePositionFromZoom(transform.position, currentZoom);
        transform.position = targetPosition;
    }

    private void Update()
    {
        // Refresh input device references if null
        if (mouse == null) mouse = Mouse.current;
        if (keyboard == null) keyboard = Keyboard.current;

        HandleKeyboardPan();
        HandleMouseDragPan();
        HandleZoom();

        // Apply bounds if enabled
        if (useBounds)
        {
            ClampToBounds();
        }
    }

    private void HandleKeyboardPan()
    {
        if (keyboard == null) return;

        Vector3 moveDir = Vector3.zero;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            moveDir += Vector3.forward;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            moveDir += Vector3.back;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            moveDir += Vector3.left;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            moveDir += Vector3.right;

        if (moveDir != Vector3.zero)
        {
            // Move on XZ plane
            Vector3 movement = moveDir.normalized * panSpeed * Time.deltaTime;
            targetPosition += new Vector3(movement.x, 0f, movement.z);
            transform.position = CalculatePositionFromZoom(targetPosition, currentZoom);
        }
    }

    private void HandleMouseDragPan()
    {
        if (mouse == null) return;

        // Start drag on middle mouse button
        if (mouse.middleButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastMousePosition = mouse.position.ReadValue();
        }

        // End drag
        if (mouse.middleButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        // Process drag
        if (isDragging)
        {
            Vector2 currentPos = mouse.position.ReadValue();
            Vector2 delta = currentPos - lastMousePosition;

            // Convert screen delta to world movement on XZ plane
            Vector3 movement = new Vector3(-delta.x, 0f, -delta.y) * dragPanSpeed * (currentZoom / 10f) * Time.deltaTime;
            targetPosition += movement;
            transform.position = CalculatePositionFromZoom(targetPosition, currentZoom);

            lastMousePosition = currentPos;
        }
    }

    private void HandleZoom()
    {
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y / 120f; // Normalize scroll value

        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentZoom -= scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            transform.position = CalculatePositionFromZoom(targetPosition, currentZoom);
        }
    }

    /// <summary>
    /// Calculate camera position from a ground target and zoom distance.
    /// </summary>
    private Vector3 CalculatePositionFromZoom(Vector3 groundTarget, float zoom)
    {
        // Camera looks down at tiltAngle, so we need to offset position
        float radAngle = tiltAngle * Mathf.Deg2Rad;
        float height = zoom * Mathf.Sin(radAngle);
        float distance = zoom * Mathf.Cos(radAngle);

        return new Vector3(
            groundTarget.x,
            height,
            groundTarget.z - distance
        );
    }

    private void ClampToBounds()
    {
        Vector3 clamped = targetPosition;
        clamped.x = Mathf.Clamp(clamped.x, cameraBounds.min.x, cameraBounds.max.x);
        clamped.z = Mathf.Clamp(clamped.z, cameraBounds.min.z, cameraBounds.max.z);

        if (clamped != targetPosition)
        {
            targetPosition = clamped;
            transform.position = CalculatePositionFromZoom(targetPosition, currentZoom);
        }
    }

    /// <summary>
    /// Set camera bounds from a grid bounds.
    /// </summary>
    public void SetBoundsFromGrid(Bounds gridBounds)
    {
        cameraBounds = gridBounds;
        useBounds = true;
    }

    /// <summary>
    /// Move camera to look at a specific world position.
    /// </summary>
    public void LookAt(Vector3 worldPosition)
    {
        targetPosition = new Vector3(worldPosition.x, 0f, worldPosition.z);
        transform.position = CalculatePositionFromZoom(targetPosition, currentZoom);
    }

    /// <summary>
    /// Set the zoom level.
    /// </summary>
    public void SetZoom(float zoom)
    {
        currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        transform.position = CalculatePositionFromZoom(targetPosition, currentZoom);
    }

    /// <summary>
    /// Get the current look-at position on the ground plane.
    /// </summary>
    public Vector3 GetLookAtPosition()
    {
        return targetPosition;
    }
}
