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
    public bool onLedgeClimb;
    public bool onRightWall;
    public bool onLeftWall;
    public bool hitCeilingCorner;
    public bool hitCeilingTopRight;
    public bool hitCeilingTopLeft;
    public bool LedgeClimbRight, LedgeClimbLeft;
    public int wallSide;


    [Space]

    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public Vector2 groundBoxSize = new Vector2(0.45f, 0.1f);
    public float ceilCollisionRadius = 0.05f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    public Vector2 topLeftOffset, topRightOffset;
    public Vector2 ledgeClimbLeftOffset, ledgeClimbRightOffset;

    private Color debugCollisionColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {  

        // onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, groundBoxSize, 0f, groundLayer);
        onLedgeClimb = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) 
                 || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        
        hitCeilingTopLeft = 
    			Physics2D.OverlapCircle((Vector2)transform.position + topLeftOffset, ceilCollisionRadius, groundLayer)
    			&&
    			(!Physics2D.OverlapCircle((Vector2)transform.position + topLeftOffset + new Vector2(2 * ceilCollisionRadius, 0), ceilCollisionRadius, groundLayer));

        hitCeilingTopRight = 
				Physics2D.OverlapCircle((Vector2)transform.position + topRightOffset, ceilCollisionRadius, groundLayer)
				&&
    			(!Physics2D.OverlapCircle((Vector2)transform.position + topRightOffset - new Vector2(2 * ceilCollisionRadius, 0), ceilCollisionRadius, groundLayer));
        hitCeilingCorner = hitCeilingTopLeft || hitCeilingTopRight;
        
        wallSide = onRightWall ? -1 : 1;


        LedgeClimbRight =
	        Physics2D.OverlapCircle((Vector2)transform.position + ledgeClimbRightOffset, collisionRadius, groundLayer);
        LedgeClimbLeft =
	        Physics2D.OverlapCircle((Vector2)transform.position + ledgeClimbLeftOffset, collisionRadius, groundLayer);

        onWall = onLedgeClimb && (LedgeClimbLeft || LedgeClimbRight);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset, groundBoxSize);
        // Gizmos.DrawWireSphere((Vector2)transform.position  + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + topLeftOffset, ceilCollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + topRightOffset, ceilCollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + ledgeClimbRightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + ledgeClimbLeftOffset, collisionRadius);
    }

}