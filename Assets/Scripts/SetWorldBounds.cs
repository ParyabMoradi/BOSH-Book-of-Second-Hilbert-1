using UnityEngine;

public class SetWorldBounds : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    public SpriteRenderer backgroundSprite; // Reference to the sprite renderer of the background image

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        if (backgroundSprite == null || mainCamera == null)
        {
            Debug.LogError("Background sprite or main camera is not assigned.");
            return;
        }

        // Calculate the bounds of the background image
        Bounds bounds = backgroundSprite.bounds;
        minBounds = bounds.min;
        maxBounds = bounds.max;

        // Calculate the camera's half dimensions
        halfHeight = mainCamera.orthographicSize;
        halfWidth = halfHeight * mainCamera.aspect;
    }

    void LateUpdate()
    {
        if (backgroundSprite == null || mainCamera == null)
        {
            return;
        }

        // Get the current camera position
        Vector3 cameraPosition = mainCamera.transform.position;

        // Clamp the camera position to the bounds of the background image
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        // Apply the clamped position to the camera
        mainCamera.transform.position = cameraPosition;
    }
}
