using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Vector2 movementVector;
    private Transform playerTransform;
    [SerializeField] private float movementSpeed;
    private Rigidbody2D playerRigidBody;

    public bool player1Left;
    public bool player1Right;
    public bool player2Left;
    public bool player2Right;

    public bool isGrounded;
    public bool player1Jumped;
    public bool player2Jumped;
    [SerializeField] private float jumpForce;

    public bool player1Slamming;
    public bool player2Slamming;
    public bool isSlamming;

    [SerializeField] private float slamForce;
    [SerializeField] private float dashSpeed;

    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] Vector2 groundedBoxCast;

    [SerializeField] float DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD = 0.2f;
    [SerializeField] float DOUBLE_TAP_TOTAL_TRESHOLD = 0.4f;

    private double rightDashStartTime = 0;
    private double rightDashEndTime = 0;

    private double leftDashStartTime = 0;
    private double leftDashEndTime = 0;
    
    private double rightArrowDashStartTime = 0;
    private double rightArrowDashEndTime = 0;

    private double leftArrowDashStartTime = 0;
    private double leftArrowDashEndTime = 0;

    public bool isDashingLeft;
    public bool isDashingRight;

    private float dashingTimeElapsed;
    [SerializeField] private float DASH_DURATION_CONST = 0.2f;
    [SerializeField] private float DASH_COOLDOWN_CONST = 4f;

    private float gravityScale;

    private float player1DashTime;
    private float player2DashTime;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        movementVector = new Vector2 (0, 0);
        playerTransform = transform;
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        gravityScale = playerRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDashingLeft || isDashingRight)
        {
            dashingTimeElapsed += Time.deltaTime;

            if (isDashingRight)
                playerRigidBody.velocity = new Vector2(dashSpeed, playerRigidBody.velocity.y);
            if (isDashingLeft)
                playerRigidBody.velocity = new Vector2(-dashSpeed, playerRigidBody.velocity.y);

            if (dashingTimeElapsed > DASH_DURATION_CONST)
            {
                isDashingLeft = false;
                isDashingRight = false;
                playerRigidBody.gravityScale = gravityScale;
                playerRigidBody.velocity = Vector2.zero;
                leftArrowDashEndTime = 0; 
                leftArrowDashStartTime = 0;
                rightArrowDashEndTime = 0; 
                rightArrowDashStartTime = 0;
                leftDashEndTime = 0; 
                leftDashStartTime = 0;
                rightDashEndTime = 0;  
                rightDashStartTime = 0;
                
            }
            return;
        }

        playerRigidBody.velocity = new Vector2(movementSpeed * movementVector.x, playerRigidBody.velocity.y);

        switch(movementVector.x)
        {
            case 0:
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsRunningRight", false);
                animator.SetBool("IsRunningLeft", false);
                break;
            case 1:
                animator.SetBool("IsWalkingRight", true);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsRunningRight", false);
                animator.SetBool("IsRunningLeft", false);
                break;
            case -1:
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsWalkingLeft", true);
                animator.SetBool("IsRunningRight", false);
                animator.SetBool("IsRunningLeft", false);   
                break;
            case 2:
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsRunningRight", true);
                animator.SetBool("IsRunningLeft", false);
                break;
            case -2:
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsRunningRight", false);
                animator.SetBool("IsRunningLeft", true);
                break;
        }


        if (playerRigidBody.velocity.y <= 0.1f && playerRigidBody.velocity.y >= -0.1f)
        {
            RaycastHit2D hit = Physics2D.BoxCast(playerTransform.position, groundedBoxCast, 0f, Vector2.down, 0.01f, groundLayerMask);
            if (hit.collider != null && hit.normal.y > 0.99f)
            {
                isGrounded = true;
                player1Jumped = false;
                player2Jumped = false;
                isSlamming = false;
            }
            else
                isGrounded = false;
        }
        else
        {
            isGrounded = false;
        }

        if(player1Slamming && player2Slamming && !isGrounded)
        {
            Slam();
        }
    }

    public void GoLeftPlayer1(InputAction.CallbackContext context)
    {
        if (context.canceled && player1Left)
        {
            player1Left = false;
            movementVector -= Vector2.left;
            leftDashEndTime = context.time;
        }

        if (isDashingLeft || isDashingRight) return;

        //if (isSlamming) return;

        if(context.performed)
        {
            movementVector += Vector2.left;
            player1Left = true;

            if (leftDashStartTime > 0)
            {
                if(leftDashEndTime - leftDashStartTime < DOUBLE_TAP_TOTAL_TRESHOLD &&
                    context.time - leftDashEndTime < DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD &&
                    Time.time - player1DashTime > DASH_COOLDOWN_CONST)
                {
                    LeftDash();
                    return;
                }
            }
            leftDashStartTime = context.time;

        }

    }

    public void GoLeftPlayer2(InputAction.CallbackContext context)
    {
        if (context.canceled && player2Left)
        {
            player2Left = false;
            movementVector -= Vector2.left;
            leftArrowDashEndTime = context.time;
        }

        if (isDashingLeft || isDashingRight) return;

        if (context.performed)
        {
            movementVector += Vector2.left;
            player2Left = true;
            if (leftArrowDashStartTime > 0)
            {
                if (leftArrowDashEndTime - leftArrowDashStartTime < DOUBLE_TAP_TOTAL_TRESHOLD &&
                    context.time - leftArrowDashEndTime < DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD &&
                    Time.time - player2DashTime > DASH_COOLDOWN_CONST)
                {
                    LeftDashArrow();

                }
            }
            leftArrowDashStartTime = context.time;
        }
        
    }

    public void GoRightPlayer1(InputAction.CallbackContext context)
    {
        if (context.canceled && player1Right)
        {
            player1Right = false;
            movementVector -= Vector2.right;
            rightDashEndTime = context.time;
        }

        if (isDashingLeft || isDashingRight) return;

        //if (isSlamming) return;

        if (context.performed)
        {
            movementVector += Vector2.right;
            player1Right = true;
            if (rightDashStartTime > 0)
            {
                if (rightDashEndTime - rightDashStartTime < DOUBLE_TAP_TOTAL_TRESHOLD &&
                    context.time - rightDashEndTime < DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD &&
                    Time.time - player1DashTime > DASH_COOLDOWN_CONST)
                {
                    RightDash();

                }
            }
            rightDashStartTime = context.time;
        }
        
    }

    public void GoRightPlayer2(InputAction.CallbackContext context)
    {
        if (context.canceled && player2Right)
        {
            player2Right = false;
            movementVector -= Vector2.right;
            rightArrowDashEndTime = context.time;
        }

        if (isDashingLeft || isDashingRight) return;

        //if (isSlamming) return;

        if (context.performed)
        {
            movementVector += Vector2.right;
            player2Right = true;
            if (rightArrowDashStartTime > 0)
            {
                if (rightArrowDashEndTime - rightArrowDashStartTime < DOUBLE_TAP_TOTAL_TRESHOLD &&
                    context.time - rightArrowDashEndTime < DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD &&
                    Time.time - player2DashTime > DASH_COOLDOWN_CONST)
                {
                    RightDashArrow();

                }
            }
            rightArrowDashStartTime = context.time;
        }
        
    }

    public void JumpPlayer1(InputAction.CallbackContext context)
    {
        if (isDashingLeft || isDashingRight) return;

        if (isSlamming || player1Jumped) return;

        if (context.performed)
        {
            player1Jumped = true;
            playerRigidBody.velocity = Vector3.zero;
            playerRigidBody.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void JumpPlayer2(InputAction.CallbackContext context)
    {
        if (isDashingLeft || isDashingRight) return;

        if (isSlamming || player2Jumped) return;

        if (context.performed)
        {
            player2Jumped = true;
            playerRigidBody.velocity = Vector3.zero;
            playerRigidBody.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void SlamPlayer1(InputAction.CallbackContext context)
    {
        if (context.performed && !isSlamming)
        {
            player1Slamming = true;
        }
        if(context.canceled)
        {
            player1Slamming = false;
        }
    }

    public void SlamPlayer2(InputAction.CallbackContext context)
    {
        if (context.performed && !isSlamming)
        {
            player2Slamming = true;
        }
        if (context.canceled)
        {
            player2Slamming = false;
        }
    }

    public void Slam()
    {
        if (isDashingLeft || isDashingRight) return;

        isSlamming = true;

        player1Slamming = false;
        player2Slamming = false;

        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.AddForce(Vector3.down * slamForce, ForceMode2D.Impulse);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Enemy"))
        {
            if(isSlamming)
            {
                Destroy(collision.gameObject);
                playerRigidBody.velocity = Vector3.zero;
                playerRigidBody.AddForce(Vector3.up * jumpForce * 0.5f, ForceMode2D.Impulse);
                player1Jumped = false;
                player2Jumped = false;
                isSlamming = false;
            }
            else if(isDashingLeft || isDashingRight)
            {
                Destroy(collision.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("JumpPad"))
        {
            if (isSlamming)
            {
                playerRigidBody.velocity = Vector3.zero;
                playerRigidBody.AddForce(Vector3.up * jumpForce * 1.5f, ForceMode2D.Impulse);
                player1Jumped = false;
                player2Jumped = false;
                isSlamming = false;
            }
        }
        if (collision.gameObject.tag.Equals("Spike"))
        {
            Destroy(gameObject);
        }
    }

    public void LeftDash()
    {
        isDashingLeft = true;
        player1DashTime = Time.time;
        dashingTimeElapsed = 0f;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.gravityScale = 0f;

        //playerRigidBody.AddForce(Vector2.left * dashForce, ForceMode2D.Impulse);
    }
    public void RightDash()
    {
        isDashingRight = true;
        player1DashTime = Time.time;
        dashingTimeElapsed = 0f;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.gravityScale = 0f;

        //playerRigidBody.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
    }
    public void LeftDashArrow()
    {
        isDashingLeft = true;
        player2DashTime = Time.time;
        dashingTimeElapsed = 0f;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.gravityScale = 0f;

        //playerRigidBody.AddForce(Vector2.left * dashForce, ForceMode2D.Impulse);

    }
    public void RightDashArrow()
    {
        isDashingRight = true;
        player2DashTime = Time.time;
        dashingTimeElapsed = 0f;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.gravityScale = 0f;


        //playerRigidBody.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
    }
}
