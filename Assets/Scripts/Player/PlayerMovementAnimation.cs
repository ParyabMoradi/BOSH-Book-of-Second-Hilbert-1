using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovementAnimation : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement move;
    private PlayerCollision coll;
    [HideInInspector]
    public SpriteRenderer sr;
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<PlayerCollision>();
        move = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        anim.SetBool("isJumping", move.isJumping);
    }
    
    public void Flip(int side)
    {

        if (move.wallGrab || move.wallSlide)
        {
            if (side == -1 && sr.flipX)
                return;

            if (side == 1 && !sr.flipX)
            {
                return;
            }
        }

        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }
}
