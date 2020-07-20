using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using MyNameSpace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private MovementState state;
    
    private int facingDirection = 1;

    public int GetDirection()
    {
        return facingDirection;
    }
    
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWallRight;
    private bool isTouchingWallLeft;
    private bool isWallSliding;
    private bool jumpPress;
    private bool jumpRelease;
    private bool canControlJump = true;
    private bool canWallJumpLeft = true;
    private bool canWallJumpRight = true;
    private bool isDashing = false;
    private float lastDash;
    private float gravityScale;
    private Vector3 m_Velocity = Vector3.zero;
    private float startAttackPress;
    private bool isHoldingDash;
    private float dashChargeCoef;
    private bool isRepushed = false;
    private float angle;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector2 stickValue;
    
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float wallSlideSpeed;
    [Header("Jump")]
    [SerializeField] private float jumpForce = 16.0f;
    [SerializeField] private float variableJumpHeightMultiplier = 0.5f;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float wallJumpCooldown;
    [SerializeField] private Vector2 wallJumpDirection;
    [Header("Distance detector")]
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float wallCheckDistance;
    [Header("Dash")]
    [SerializeField] private float dashForce = 400f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashChargeTimeMax = 1f; //En seconde
    [SerializeField] private float dashChargeHoldMax = 0f; //En second
    [Header("Transform")]
    public Transform groundCheck;
    public Transform wallCheckRight;
    public Transform wallCheckLeft;
    public LayerMask whatIsGround;
    

    // Start is called before the first frame update
    void Start()
    {
        state = MovementState.IDLE;
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        wallJumpDirection.Normalize();
        gravityScale = rb.gravityScale;
        dashChargeHoldMax += dashChargeTimeMax;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        //UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        ForcedDash();
        jumpPress = false;
        jumpRelease = false;
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckIfWallSliding()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            if (isTouchingWallRight && stickValue.x > 0)
            {
                isWallSliding = true;
            }
            else if (isTouchingWallLeft && stickValue.x < 0)
            {
                isWallSliding = true;
            }
            else
            {
                isWallSliding = false;
            }
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWallRight = Physics2D.Raycast(wallCheckRight.position, transform.right, wallCheckDistance, whatIsGround);
        isTouchingWallLeft = Physics2D.Raycast(wallCheckLeft.position, -transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0 || isWallSliding)
        {
            canControlJump = true;
        }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && stickValue.x < 0)
        {
            Flip();
        }
        else if (!isFacingRight && stickValue.x > 0)
        {
            Flip();
        }
    }

    /*private void UpdateAnimations()
    {
        anim.SetBool("Walking", !Mathf.Approximately(stickValue.x, 0F));
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("Sliding", isWallSliding);
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("HoldingDash", isHoldingDash);
        anim.SetFloat("dashTimer", (Time.time - startAttackPress - dashChargeHoldMax) );
    }*/

    private void CheckInput()
    {
        if (jumpPress)
        {
            Jump();
        }

        if (jumpRelease && rb.velocity.y > 0 && canControlJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

    }

    private void Jump()
    {
        if (isGrounded && !isWallSliding && !isHoldingDash) // Saut normal
        {
            Vector2 jumpforce = new Vector2(rb.velocity.x, jumpForce);
            rb.AddForce(jumpforce, ForceMode2D.Impulse);
        }
        else if (!isGrounded && !isHoldingDash) //walljump
        {
            isWallSliding = false;

            if (isTouchingWallRight && canWallJumpRight)
            {
                canWallJumpRight = false;
                canWallJumpLeft = true;
                canControlJump = false;

                rb.velocity = new Vector2(0f, 0f);
                Vector2 forceToAdd = new Vector2(wallJumpForce * -wallJumpDirection.x, wallJumpForce * wallJumpDirection.y);
                rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                //Smooth le vecteur
                Invoke("ResetWallJumpRight", wallJumpCooldown);
            }
            else if (isTouchingWallLeft && canWallJumpLeft)
            {
                canWallJumpRight = true; 
                canWallJumpLeft = false;
                canControlJump = false;

                rb.velocity = new Vector2(0, 0);
                Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x, wallJumpForce * wallJumpDirection.y);
                rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                Invoke("ResetWallJumpLeft", wallJumpCooldown);
            }
        }
    }

    private void ResetWallJumpLeft()
    {
        canWallJumpLeft = true;
    }
    private void ResetWallJumpRight()
    {
        canWallJumpRight = true;
    }

    private void ApplyMovement()
    {
        if(canWallJumpLeft && canWallJumpRight && (!isDashing && lastDash + dashTime < Time.time) && !isHoldingDash && !isRepushed)
        {
            rb.velocity = new Vector2(movementSpeed * stickValue.x, rb.velocity.y);
            state = MovementState.MOVE;
        }

        if (isWallSliding && rb.velocity.y < -wallSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            state = MovementState.MOVE;
        }
    }

    private void ForcedDash()
    {
        if((Time.time - startAttackPress) > dashChargeHoldMax && isHoldingDash)
        {
            isHoldingDash = false;
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        state = MovementState.DASH;
        dashChargeCoef = Mathf.Clamp(Time.time - startAttackPress, 0, dashChargeTimeMax)/dashChargeTimeMax;

        lastDash = Time.time;
        isDashing = true;
        rb.gravityScale = 0f;

        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180);
        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180);
        
        rb.velocity = new Vector2(dashForce * dashChargeCoef * xcomponent, dashForce * dashChargeCoef * ycomponent);
        
        yield return new WaitForSeconds(dashTime);
        
        isDashing = false;
        rb.gravityScale = gravityScale;

        Vector3 targetVelocity = new Vector2(0, 0);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, 0);
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            sprite.flipX = !sprite.flipX;
        }
    }

    private void OnDashPress()
    {
        if (lastDash + dashCooldown < Time.time)
        {
            state = MovementState.PREPDASH;
            rb.velocity = Vector3.zero;
            rb.gravityScale = 0.1f;
            startAttackPress = Time.time;
            isHoldingDash = true;
        }
    }
    private void OnDashRelease()
    {
        if (isHoldingDash)
        {
            isHoldingDash = false;
            StartCoroutine(Dash());
        }
    }

    private void OnJumpPress()
    {
        jumpPress = true;
    }

    private void OnJumpRelease()
    {
        jumpRelease = true;
    }

    private void OnMovement(InputValue value)
    {
        stickValue = value.Get<Vector2>();
        angle = Mathf.Atan2(stickValue.y, stickValue.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(stickValue.y) < 0.3f)
        {
            stickValue.y = 0.0f;

        }
        if (Mathf.Abs(stickValue.x) < 0.3f)
        {
            stickValue.x = 0.0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheckRight.position, new Vector3(wallCheckRight.position.x + wallCheckDistance, wallCheckRight.position.y, wallCheckRight.position.z));
        Gizmos.DrawLine(wallCheckLeft.position, new Vector3(wallCheckLeft.position.x - wallCheckDistance, wallCheckLeft.position.y, wallCheckLeft.position.z));
    }

}

    