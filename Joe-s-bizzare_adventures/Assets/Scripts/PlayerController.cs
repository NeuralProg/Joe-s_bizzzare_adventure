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

    [Header("Dash")]
    [SerializeField] private GameObject dashEffect;
    [SerializeField] private GameObject dashResetAnim;
    private GameObject dashResetEffectObject;
    private bool isDashing = false;
    private float dashMultiplier = 2.5f;
    private float dashTime = 0.2f;
    private float dashCooldown = 0.3f;
    private bool isInDashCooldown;
    private bool dashReset;
    private bool canDash;

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
        if (!isDashing)
        {
            MoveHorizontaly();
            Jump();
        }
        else
        {
            rb.velocity = new Vector2(transform.localScale.x * moveSpeed * dashMultiplier, 0f);
        }
        Dash();
        
        // Checks
        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, 0.15f, whatIsGround);
        isFalling = rb.velocity.y < -0.1f;
        if(isGrounded)
        {
            canDoubleJump = true;
            if(!jumping)
                jumpCoyoteTimer = jumpCoyoteTime;
            dashReset = true;
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

    private void Dash()
    {
        // dash
        canDash = !isDashing && !isInDashCooldown && dashReset;

        if (dashResetEffectObject != null)
            dashResetEffectObject.transform.position = transform.position;

        if ((Input.GetKeyDown(KeyCode.LeftShift)) && canDash)
        {
            StartCoroutine(DashTime());
        }
    }
    private IEnumerator DashTime()
    {
        isDashing = true;
        dashReset = false;
        rb.gravityScale = 0f;
        for (int i = 0; i < 10; i++) 
        {
            yield return new WaitForSeconds(dashTime/10);
            Instantiate(dashEffect, groundCheckPos.position, groundCheckPos.rotation);
        }
        isDashing = false;
        rb.gravityScale = 5f;
        isInDashCooldown = true;

        yield return new WaitForSeconds(dashCooldown);

        if(isGrounded)
        {
            dashResetEffectObject = Instantiate(dashResetAnim, transform.position, transform.rotation);
        }
        isInDashCooldown = false;
    }

    #endregion
}
