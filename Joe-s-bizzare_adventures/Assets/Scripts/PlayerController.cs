using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
    - isGrounded & material no friction
    - Fonctions et #regions
    - jump coyote time
    - Jump anim --> animator Trigger
    - 
*/

public class PlayerController : MonoBehaviour
{
    // Movements
    private float moveSpeed = 5f;
    private bool jumping = false;
    private float jumpHeight = 20f;
    private bool canDoubleJump; //---------------------------------------

    [Header("Checks")]
    [SerializeField] private UnityEngine.Transform groundCheckPos;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    // References
    private Rigidbody2D rb;
    private Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // x movement 
        var xDirection = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(xDirection * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        // Jump
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheckPos.position, 0.15f, whatIsGround);

        // flip
        if (rb.velocity.x < -0.1f)
        {
            transform.localScale = new Vector3(-1f, 1, 1);
        }
        else if(rb.velocity.x > 0.1f)
        {
            transform.localScale = Vector3.one;
        }

        // Limit falling speed
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -30, 30));
    }
}
