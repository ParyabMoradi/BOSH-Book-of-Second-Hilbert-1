using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    private PlayerCollision coll;
    private Rigidbody2D rb;
    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
    public float nudgeStrength = 0.05f;
    [Space]
    [Header("Booleans")]
    public bool canMove = true;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;
    public bool isJumping;
    [Space]
    private bool groundTouch;
    private bool hasDashed;
    public int side = 1;

    
    [Space]
    float fJumpPressedRemember = 0;
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;
    
    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f;

    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingBasic = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    float fCutJumpHeight = 0.5f;

    private float maxFallSpeed = 20;
    
    Vector2 originalVelocity;
    // private bool hittedCeiling = false;

    void Start()
    {
        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        if (isJumping && !coll.onGround)
        {
            if (rb.linearVelocity.y > 0.5f)
                originalVelocity = rb.linearVelocity;
            // hittedCeiling = true;
        }

        fGroundedRemember -= Time.deltaTime;
        if (coll.onGround)
        {
            fGroundedRemember = fGroundedRememberTime;
        }
        fJumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            fJumpPressedRemember = fJumpPressedRememberTime;
        }
        
        Walk(dir);
        
        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            // if(side != coll.wallSide)
            //     anim.Flip(side*-1);
            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<PlayerBetterJumping>().enabled = true;
        }
        
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }
        if(coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;
        
        
        // if (coll.onGround)
        if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            Jump(Vector2.up, false);
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * fCutJumpHeight);
            }
        }
        
        if (coll.hitCeilingCorner && !(coll.hitCeilingTopLeft && coll.hitCeilingTopRight) && isJumping)
        {
            // Debug.Log(originalVelocity);
            if (coll.hitCeilingTopLeft)
            {
                rb.MovePosition(rb.position + new Vector2(nudgeStrength, 0));
            }
            else if (coll.hitCeilingTopRight)
            {
                rb.MovePosition(rb.position + new Vector2(-nudgeStrength, 0));
            }

            rb.linearVelocity = originalVelocity;
        }

        
        if (Input.GetButtonDown("Jump") && coll.onWall && !coll.onGround)
        {
            // anim.SetTrigger("jump");
            WallJump();
        }

        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        // WallParticle(y);

        if (wallGrab || wallSlide || !canMove)
            return;

        if(x > 0)
        {
            side = 1;
            // anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            // anim.Flip(side);
        }


    }
    
    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            // rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
            
            
            float fHorizontalVelocity = rb.linearVelocity.x;
            fHorizontalVelocity += dir.x;
            
            if (Mathf.Abs(dir.x) < 0.01f)
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
            else if (Mathf.Sign(dir.x) != Mathf.Sign(fHorizontalVelocity))
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime);
            else
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);
            if (Mathf.Abs(fHorizontalVelocity) < speed)
                rb.linearVelocity = new Vector2(fHorizontalVelocity, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(side * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(dir.x * speed, rb.linearVelocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }
    
    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
        isJumping = false;

        // side = anim.sr.flipX ? -1 : 1;

        // jumpParticle.Play();
    }
    
    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        // FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        // anim.SetTrigger("dash");

        rb.linearVelocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.linearVelocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }
    
    IEnumerator DashWait()
    {
        // FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        // dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<PlayerBetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        // dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<PlayerBetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            // anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
        isJumping = true;
    }
    
    private void WallSlide()
    {
        // if(coll.wallSide != side)
        //     anim.Flip(side * -1);

        if (!canMove)
            return;

        bool pushingWall = false;
        if((rb.linearVelocity.x > 0 && coll.onRightWall) || (rb.linearVelocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.linearVelocity.x;

        rb.linearVelocity = new Vector2(push, -slideSpeed);
    }
    
    private void Jump(Vector2 dir, bool wall)
    {
        // slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        // ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * jumpForce;

        isJumping = true;

        // particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.linearDamping = x;
    }


}