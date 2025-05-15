using UnityEngine;
using Unity.Netcode;

public class CrosshairCursor : NetworkBehaviour
{
    public Color defaultColor = Color.white;
    public Color hoverColor = Color.red;
    public LayerMask enemyLayer;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false); // Disable this crosshair for non-owners
            return;
        }

        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on CrosshairCursor GameObject.");
        }
    }

    void Update()
    {
        if (!IsOwner || spriteRenderer == null) return;

        if (Camera.main == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f, enemyLayer);

        spriteRenderer.color = hit.collider != null ? hoverColor : defaultColor;
    }
}
