using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBetterJumping : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerCollision coll;
    private PlayerMovement playerMovement;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float jumpHangGravityMult = 0.25f;
    public float jumpHangTimeThreshold = 2f;
    public float peakJumpHangGravityMult = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PlayerCollision>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void FixedUpdate()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }else if(rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if (playerMovement.isJumping && Mathf.Abs(rb.linearVelocity.y) < jumpHangTimeThreshold)
        {
            rb.gravityScale *= peakJumpHangGravityMult;
        }
    }
}