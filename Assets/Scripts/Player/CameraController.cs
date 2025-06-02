using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    public float zoomSpeed = 2f;
    public Vector3 offset = new Vector3(0, 4, -10);

    private Transform target;
    private float targetZoom;
    private float defaultZoom;
    private bool isLockedToArea = false;
    private Bounds lockBounds;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (cam != null)
            defaultZoom = cam.orthographicSize;

        targetZoom = defaultZoom;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Clamp the desired position before smoothing
        if (isLockedToArea && cam != null)
        {
            desiredPosition = ClampPositionToBoundsSoft(desiredPosition);
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        smoothedPosition.z = offset.z; // Always keep Z fixed

        transform.position = smoothedPosition;

        if (cam != null)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }

    public void SetCameraTarget(Transform newTarget)
    {
        Debug.Log($"Setting camera target to: {newTarget?.name}");
        target = newTarget;

    }

    public void FocusOnTarget(Transform focusTarget, float zoomOutSize = 8f, Bounds? areaBounds = null)
    {
        target = focusTarget;
        targetZoom = zoomOutSize;

        if (areaBounds.HasValue)
        {
            isLockedToArea = true;
            lockBounds = areaBounds.Value;
        }
    }

    public void ResetFocus()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        targetZoom = defaultZoom;
        isLockedToArea = false;
    }

    // Soft Clamp - allows camera to approach edges but not overshoot
    private Vector3 ClampPositionToBoundsSoft(Vector3 position)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.aspect * camHeight;

        float minX = lockBounds.min.x + camWidth;
        float maxX = lockBounds.max.x - camWidth;
        float minY = lockBounds.min.y + camHeight;
        float maxY = lockBounds.max.y - camHeight;

        Vector3 clamped = position;
        clamped.x = Mathf.Clamp(position.x, minX, maxX);
        clamped.y = Mathf.Clamp(position.y, minY, maxY);

        return clamped;
    }
}
