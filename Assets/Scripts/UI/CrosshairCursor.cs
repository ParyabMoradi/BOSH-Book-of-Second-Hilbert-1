using UnityEngine;

public class CrosshairCursor : MonoBehaviour
{
    public Color defaultColor = Color.white;
    public Color hoverColor = Color.red;
    public LayerMask enemyLayer;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;

        // Check if the cursor is over an enemy using 2D raycast
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f, enemyLayer);

        if (hit.collider != null)
        {
            spriteRenderer.color = hoverColor;
        }
        else
        {
            spriteRenderer.color = defaultColor;
        }
    }
}
