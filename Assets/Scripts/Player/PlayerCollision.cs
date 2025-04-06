using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool hitCeilingCorner;
    public bool hitCeilingTopRight;
    public bool hitCeilingTopLeft;
    public int wallSide;


    [Space]

    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    public Vector2 topLeftOffset, topRightOffset;

    private Color debugCollisionColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {  
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) 
                 || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        
        hitCeilingTopLeft = Physics2D.OverlapCircle((Vector2)transform.position + topLeftOffset, collisionRadius, groundLayer);
        hitCeilingTopRight = Physics2D.OverlapCircle((Vector2)transform.position + topRightOffset, collisionRadius, groundLayer);
        hitCeilingCorner = hitCeilingTopLeft || hitCeilingTopRight;
        
        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position  + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + topLeftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + topRightOffset, collisionRadius);

    }
    
    public float GetHorizontalNudgeDistance(bool hittingLeftCorner)
    {
        float maxNudge = 0.5f; // Max distance we’ll try nudging
        float step = 0.01f;    // Precision of the check

        Vector2 origin = (Vector2)transform.position;
        Vector2 direction = hittingLeftCorner ? Vector2.right : Vector2.left;

        // Try shifting step by step until we no longer hit the platform
        for (float dist = step; dist <= maxNudge; dist += step)
        {
            Vector2 checkPos = origin + direction * dist + (hittingLeftCorner ? topLeftOffset : topRightOffset);

            bool stillHitting = Physics2D.OverlapCircle(checkPos, collisionRadius, groundLayer);

            if (!stillHitting)
            {
                return dist;
            }
        }

        // Default fallback
        return maxNudge;
    }

}