using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private Animator[] anims;
    private Animator anim;
    private SpriteRenderer[] sprites;
    private SpriteRenderer spriteRenderer;
    private Transform child;
    private ClientNetworkTransform[] clientNetTransforms;
    public bool resetPosition = true;



    private PlayerCollision coll;
    private Rigidbody2D rb;
    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 15;
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
    public bool pickedUpTheBook = false;
    private Transform heldBook;
    private bool isHoldingLedge = false;
    private float ledgeHoldTimer = 0f;
    private const float maxLedgeHoldTime = 0.2f;

    [Space]
    private bool groundTouch;
    private bool hasDashed;
    public int side = 1;

    [Header("Crouch Settings")]
    public bool isCrouching = false;
    public float crouchSpeedMultiplier = 0.5f;


    [Space]
    float fJumpPressedRemember = 0;
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;
    private float jumpCooldownTimer = 0f;


    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f;

    public float wallGrabOppositeReleaseTime = 0.2f;
    private float oppositeInputTimer = 0f;


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
    public float wallGrabHoldTime = 0.1f;
    float wallGrabTimer = 0f;
    int moveInput = 0;
    int previousInput = 0;
    bool isHoldingToWall = false;

    Vector2 originalVelocity;
    // private bool hittedCeiling = false;

    private Collider2D playerCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    public Vector2 crouchColliderSize = new Vector2(0.5f, 0.5f);
    public Vector2 crouchColliderOffset = new Vector2(0f, -0.25f);

    public override void OnNetworkSpawn()
    {
        anims = GetComponentsInChildren<Animator>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        CharacterType role = RoleManager.Instance.GetOrAssignRole(OwnerClientId);
        // clientNetTransforms = GetComponentsInChildren<ClientNetworkTransform>();
        Debug.Log(role);

        if (role == CharacterType.Boy)
        {
            
            child = transform.GetChild(0);
            anim = anims[0];
            spriteRenderer = sprites[1];
            anims[1].enabled = false;
            sprites[2].enabled = false;
            sprites[0].enabled = false;
            // clientNetTransforms[2].enabled = false;
        }
        else
        {
            child = transform.GetChild(1);
            anim = anims[1];
            spriteRenderer = sprites[2];
            anims[0].enabled = false;
            sprites[1].enabled = false;
            sprites[0].enabled = false;

            // clientNetTransforms[1].enabled = false;

        }
        

        if (!IsOwner)
        {

            enabled = false;
            return;
        }
        
    }

    void Start()
    {
        transform.position = Vector3.zero;
        coll = GetComponent<PlayerCollision>();
        rb = GetComponent<Rigidbody2D>();
        

		playerCollider = GetComponent<Collider2D>();
		if (playerCollider is BoxCollider2D box)
		{
    		originalColliderSize = box.size;
    		originalColliderOffset = box.offset;
		}
		else if (playerCollider is CapsuleCollider2D capsule)
		{
    		originalColliderSize = capsule.size;
    		originalColliderOffset = capsule.offset;
		}

    }

    void Update()
    {
        if (resetPosition)
        {
            resetPosition = false;
            rb.position = new Vector2(0, 0);
            rb.rotation = 0;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
        // if (!IsOwner || MatchManager.Instance == null || MatchManager.Instance.isGameFrozen.Value)
        // return;

        if (Time.timeScale == 0) return;
        
        // child.localScale = new Vector3(side * Math.Abs(child.localScale.x), child.localScale.y, child.localScale.z);
        // spriteRenderer.flipX = side == -1;
        anim.SetBool("flip",side==-1);
        anim.SetFloat("v_y",rb.linearVelocityY);
        anim.SetFloat("v_x",MathF.Abs(rb.linearVelocityX));
        anim.SetBool("wallGrab",wallGrab);
        anim.SetBool("onGround", coll.onGround);
        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);
        
        Vector2 moveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            moveDirection.y += 1;
        if (Input.GetKey(KeyCode.S))
            moveDirection.y -= 1;
        if (Input.GetKey(KeyCode.A))
            moveDirection.x -= 1;
        if (Input.GetKey(KeyCode.D))
            moveDirection.x += 1;

        if (x > 0)
            moveInput = 1;
        else if (x < 0)
            moveInput = -1;
        else
            moveInput = 0;
        
        if (coll.onGround && Input.GetAxisRaw("Vertical") < 0)
{
    if (!isCrouching)
    {
        isCrouching = true;
        if (playerCollider is BoxCollider2D box)
        {
            box.size = crouchColliderSize;
            box.offset = crouchColliderOffset;
        }
        else if (playerCollider is CapsuleCollider2D capsule)
        {
            capsule.size = crouchColliderSize;
            capsule.offset = crouchColliderOffset;
        }
    }
}
else
{
    if (isCrouching)
    {
        isCrouching = false;
        if (playerCollider is BoxCollider2D box)
        {
            box.size = originalColliderSize;
            box.offset = originalColliderOffset;
        }
        else if (playerCollider is CapsuleCollider2D capsule)
        {
            capsule.size = originalColliderSize;
            capsule.offset = originalColliderOffset;
        }
    }
}

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
		jumpCooldownTimer -= Time.deltaTime;

        
        Walk(dir,moveDirection);
        
        if ((coll.onWall || coll.onLedgeClimb) && canMove)
			{
    if (moveInput == side)
    {
        if (previousInput == moveInput)
            wallGrabTimer += Time.deltaTime;
        else
            wallGrabTimer = 0f;

        if (wallGrabTimer >= wallGrabHoldTime)
        {
            wallGrab = true;
            wallSlide = false;
        }

        previousInput = moveInput;
        oppositeInputTimer = 0f;
    }
    else if (moveInput == -side)
    {
        oppositeInputTimer += Time.deltaTime;

        if (oppositeInputTimer >= wallGrabOppositeReleaseTime)
        {
            wallGrab = false;
            wallSlide = false;
            wallGrabTimer = 0f;
        }
    }
    else
    {
        //wallGrabTimer = 0f;
        oppositeInputTimer = 0f;
    }
}
else if (wallGrab)
{
    wallGrab = false;
    wallSlide = false;
    wallGrabTimer = 0f;
    previousInput = 0;
    oppositeInputTimer = 0f;
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

        anim.speed = 1;
        if ((wallGrab || coll.onLedgeClimb) && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            float speedModifier = 0.5f;
			if (Input.GetAxisRaw("Vertical") < 0)
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slideSpeed);
			else if (!(!coll.onWall && coll.onLedgeClimb))
            	rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Abs(x) * (speed * speedModifier));
            if (!coll.onWall && coll.onLedgeClimb && !coll.onGround)
            {
                anim.speed = 0;
            }
            else
                anim.speed = Mathf.Abs(rb.linearVelocityY)/slideSpeed;
                
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
        
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (!(coll.onWall || coll.onLedgeClimb) || coll.onGround)
            wallSlide = false;
        
        if (Input.GetButtonDown("Jump") && (coll.onWall || (!coll.onWall && coll.onLedgeClimb && ((coll.onRightWall && moveInput != 1) || (coll.onLeftWall && moveInput != -1)))) && !coll.onGround)
        {
            // anim.SetTrigger("jump");
            WallJump();
        }else if (Input.GetButtonDown("Jump") && !coll.onWall && coll.onLedgeClimb && ((coll.onRightWall && moveInput == 1) || (coll.onLeftWall && moveInput == -1)) && !coll.onGround)
        {
            // anim.SetTrigger("ledgeClimb");
            LedgeClimb();
        }else if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0) && jumpCooldownTimer <= 0f)
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            Jump(Vector2.up, false);
 			jumpCooldownTimer = 2*fJumpPressedRememberTime;
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

        
        

        if (Input.GetButtonDown("Fire3") && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
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


        if (IsLocalPlayer) // only local player runs input
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPickOrDropBookServerRpc();
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void TryPickOrDropBookServerRpc(ServerRpcParams rpcParams = default)
    {
        if (pickedUpTheBook) // your bool that tracks if this player holds the book
        {
            DropBook();
        }
        else
        {
            TryPickUpBook();
        }
    }
    void TryPickUpBook()
    {
        Debug.Log("here");
        // You can use a trigger or physics overlap to detect nearby book
        Collider2D bookCollider = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("TheBook"));

        if (bookCollider != null && bookCollider.CompareTag("TheBook"))
        {
            Debug.Log("here inside");
            Transform bookRoot = bookCollider.transform.parent ?? bookCollider.transform;
            Rigidbody2D bookRb = bookRoot.GetComponent<Rigidbody2D>();

            if (bookRb != null)
            {
                bookRb.isKinematic = true;
                bookRb.linearVelocity = Vector2.zero;
                bookRoot.SetParent(transform);
                bookRoot.localPosition = new Vector3(0, 1f, 0);

                heldBook = bookRoot;
                pickedUpTheBook = true;
            }
        }
    }

    void DropBook()
    {
        if (heldBook != null)
        {
            heldBook.SetParent(null);
            Rigidbody2D bookRb = heldBook.GetComponent<Rigidbody2D>();
            if (bookRb != null)
            {
                bookRb.isKinematic = false;
            }

            heldBook = null;
            pickedUpTheBook = false;
        }
    }



    
    private void Walk(Vector2 dir, Vector2 moveDirection)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        float currentSpeed = isCrouching ? speed * crouchSpeedMultiplier : speed;

        if (!wallJumped)
        {
            float fHorizontalVelocity = rb.linearVelocity.x;
            fHorizontalVelocity += dir.x;

            if (moveDirection.x == 0)
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
            else if (Mathf.Sign(dir.x) != Mathf.Sign(fHorizontalVelocity))
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
            else
                fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

            if (Mathf.Abs(fHorizontalVelocity) < currentSpeed)
                rb.linearVelocity = new Vector2(moveDirection.x * Mathf.Abs(fHorizontalVelocity), rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(moveDirection.x * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(dir.x * currentSpeed, rb.linearVelocity.y), wallJumpLerp * Time.deltaTime);
        }
    }

    
    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
        isJumping = false;
		anim.SetBool("isJumping",false);

        // side = anim.sr.flipX ? -1 : 1;

        // jumpParticle.Play();
    }
    
    private void Dash(float x, float y)
    {
        // Camera.main.transform.DOComplete();
        // Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
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
		anim.SetBool("isJumping",true);
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            // anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.15f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
        isJumping = true;
 		jumpCooldownTimer = 2*fJumpPressedRememberTime;
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

		anim.SetBool("isJumping",true);
        
        //rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * jumpForce;

        isJumping = true;

        // particle.Play();
    }

private void LedgeClimb()
{
    if (!canMove || isDashing)
        return;

	fJumpPressedRemember = 0;
    StartCoroutine(PerformLedgeClimb());
}

IEnumerator PerformLedgeClimb()
{
    canMove = false;
    rb.linearVelocity = Vector2.zero;
    rb.gravityScale = 0;

    // Optional: Trigger ledge climb animation
    // anim.SetTrigger("ledgeClimb");
	
	Vector2 ledgeClimbHoldPosition = (Vector2)transform.position + (side==1 ? coll.ledgeClimbRightOffset+new Vector2(0.1f,0.1f) : coll.ledgeClimbLeftOffset+new Vector2(-0.1f,0.1f));
    // Move to a holding point if needed before the full climb (you can skip this if not necessary)
    Vector2 holdPosition = ledgeClimbHoldPosition; // Assume this is defined in your PlayerCollision
    transform.position = holdPosition;

    // Wait for animation timing or a short pause
    yield return new WaitForSeconds(0.2f);

    // Move the player to the top of the ledge
    //Vector2 climbUpPosition = ledgeClimbHoldPosition; // This should be the "final" ledge top position
    //transform.position = climbUpPosition;

    //yield return new WaitForSeconds(0.2f); // Optional: match this to the length of the animation

    canMove = true;
    rb.gravityScale = 3;

    // Optionally, reset any wall states
    wallGrab = false;
    wallSlide = false;
    wallJumped = false;
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