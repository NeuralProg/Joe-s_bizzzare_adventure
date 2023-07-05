using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
    - Fonctions et #regions
    - 
*/

public class PlayerController : MonoBehaviour
{
    #region Variables

    // X Move
    private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private GameObject doubleJumpEffect;
    private bool jumping = false;
    private float jumpHeight = 18f;
    private float jumpCoyoteTime = 0.2f;
    private float jumpCoyoteTimer;
    private bool canDoubleJump;

    [Header("Checks")]
    [SerializeField] private UnityEngine.Transform groundCheckPos;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isFalling;

    // References
    private Rigidbody2D rb;
    private Animator anim;

    #endregion


    #region Default

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        MoveHorizontaly();
        Jump();
        
        // Checks
        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, 0.15f, whatIsGround);
        isFalling = rb.velocity.y < -0.1f;
        if(isGrounded)
        {
            canDoubleJump = true;
            if(!jumping)
                jumpCoyoteTimer = jumpCoyoteTime;
        }
        else
        {
            jumpCoyoteTimer -= Time.deltaTime;
        }
        if (isFalling) 
        {
            jumping = false;
        }

        // Limit falling speed
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -30, 30));

        // Anims
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("Jumping", jumping);
        anim.SetBool("Falling", isFalling);
    }

    #endregion


    #region Functions

    private void MoveHorizontaly()
    {
        // x movement 
        var xDirection = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(xDirection * moveSpeed, rb.velocity.y);

        // flip
        if (rb.velocity.x < -0.1f)
        {
            transform.localScale = new Vector3(-1f, 1, 1);
        }
        else if (rb.velocity.x > 0.1f)
        {
            transform.localScale = Vector3.one;
        }
    }

    private void Jump()
    {
        // Jump
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && (isGrounded || jumpCoyoteTimer > 0))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            jumping = true;
            jumpCoyoteTimer = -1f;
            anim.SetTrigger("Jump");
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && canDoubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            Instantiate(doubleJumpEffect, groundCheckPos.position, groundCheckPos.rotation);
            canDoubleJump = false;
            jumping = true;
            anim.SetTrigger("Jump");
        }
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) && !isFalling))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 3);
            jumping = false;
        }
    }

    #endregion
}
