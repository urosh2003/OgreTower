using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private Vector2 movementVector;
    public Transform playerTransform;
    [SerializeField] public float walkingSpeed;
    [SerializeField] public float runningSpeed;
    public Rigidbody2D playerRigidBody;

    public bool player1Left;
    public bool player1Right;
    public bool player2Left;
    public bool player2Right;

    public bool isGrounded;
    public bool player1Jumped;
    public bool player2Jumped;
    [SerializeField] public float jumpForce;

    public bool player1Slamming;
    public bool player2Slamming;
    public bool isSlamming;

    [SerializeField] public float slamForce;
    [SerializeField] public float dashSpeed;

    [SerializeField] public LayerMask groundLayerMask;
    [SerializeField] public Vector2 groundedBoxCast;

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

    public float gravityScale;
    public float fallingGravity;

    private float player1DashTime;
    private float player2DashTime;

    public Animator animator;
    public static event Action movementDirectionChanged;
    public static event Action jumped;
    public static event Action<float> hasGrounded;
    public bool canDash;

    private void Awake()
    {
        Instance = this;
    }

    public MovementDirectionState movementDirection;
    public MovementTypeState movementType;

    // Start is called before the first frame update
    void Start()
    {
        movementVector = new Vector2 (0, 0);
        playerTransform = transform;
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        movementDirection = new IdleState();
        movementDirection.Enter();
        movementType = new GroundedState();
        movementDirection.Enter();

    }

    // Update is called once per frame
    void Update()
    {
        if (isDashingLeft || isDashingRight)
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
                //playerRigidBody.velocity = Vector2.zero;
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
        else
        {
            movementDirection.Update();
            MovementTypeState newState = movementType.Update();
            if (newState != null)
            {
                movementType.Exit();
                movementType = newState;
                movementType.Enter();
            }
        }
    }

    public void Grounded()
    {
        hasGrounded.Invoke(transform.position.y);
    }

    public void GoLeft(InputAction.CallbackContext context)
    {
        MovementDirectionState newState = movementDirection.GoLeft(context);
        if(newState != null)
        {
            movementDirection.Exit();
            movementDirection = newState;
            movementDirection.Enter();
            movementDirectionChanged.Invoke();
        }
    }
    public void GoRight(InputAction.CallbackContext context)
    {
        MovementDirectionState newState = movementDirection.GoRight(context);
        if (newState != null)
        {
            movementDirection.Exit();
            movementDirection = newState;
            movementDirection.Enter();
            movementDirectionChanged.Invoke();
        }
    }

    public void GoLeftPlayer1(InputAction.CallbackContext context)
    {
        if (canDash)
            CheckForDashLeft1(context);
        GoLeft(context);
    }
    private void CheckForDashLeft1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (leftDashStartTime > 0 &&
               leftDashEndTime - leftDashStartTime < DOUBLE_TAP_TOTAL_TRESHOLD &&
               context.time - leftDashEndTime < DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD &&
               Time.time - player1DashTime > DASH_COOLDOWN_CONST)
                LeftDash();
            leftDashStartTime = context.time;
        }
        if (context.canceled)
            leftDashEndTime = context.time;
    }

    public void GoLeftPlayer2(InputAction.CallbackContext context)
    {
        if (canDash)
            CheckForDashLeft2(context);
        GoLeft(context);
    }
    private void CheckForDashLeft2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (leftArrowDashStartTime > 0 &&
               leftArrowDashEndTime - leftArrowDashStartTime < DOUBLE_TAP_TOTAL_TRESHOLD &&
               context.time - leftArrowDashEndTime < DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD &&
               Time.time - player2DashTime > DASH_COOLDOWN_CONST)
                LeftDashArrow();
            leftArrowDashStartTime = context.time;
        }
        if (context.canceled)
            leftArrowDashEndTime = context.time;
    }

    public void GoRightPlayer1(InputAction.CallbackContext context)
    {
        if (canDash)
            CheckForDashRight1(context);
        GoRight(context);
    }
    private void CheckForDashRight1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (rightDashStartTime > 0 &&
               rightDashEndTime - rightDashStartTime < DOUBLE_TAP_TOTAL_TRESHOLD &&
               context.time - rightDashEndTime < DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD &&
               Time.time - player1DashTime > DASH_COOLDOWN_CONST)
                RightDash();
            rightDashStartTime = context.time;
        }
        if (context.canceled)
            rightDashEndTime = context.time;
    }
    public void GoRightPlayer2(InputAction.CallbackContext context)
    {
        if (canDash)
            CheckForDashRight2(context);
        GoRight(context);
    }
    private void CheckForDashRight2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (rightArrowDashStartTime > 0 &&
               rightArrowDashEndTime - rightArrowDashStartTime < DOUBLE_TAP_TOTAL_TRESHOLD &&
               context.time - rightArrowDashEndTime < DOUBLE_TAP_BETWEEN_TAPS_TRESHOLD &&
               Time.time - player2DashTime > DASH_COOLDOWN_CONST)
                RightDashArrow();
            rightArrowDashStartTime = context.time;
        }
        if (context.canceled)
            rightArrowDashEndTime = context.time;
    }

    public void JumpPlayer1(InputAction.CallbackContext context)
    {
        if (isDashingLeft || isDashingRight) return;

        MovementTypeState newState = movementType.JumpPlayer1(context);
        if (newState != null)
        {
            movementType.Exit();
            movementType = newState;
            movementType.Enter();
            jumped.Invoke();
        }
    }

    public void JumpPlayer2(InputAction.CallbackContext context)
    {
        if (isDashingLeft || isDashingRight) return;

        MovementTypeState newState = movementType.JumpPlayer2(context);
        if (newState != null)
        {
            movementType.Exit();
            movementType = newState;
            movementType.Enter();
            jumped.Invoke();
        }
    }

    public void SlamPlayer1(InputAction.CallbackContext context)
    {
        MovementTypeState newState = movementType.SlamPlayer1(context);
        if (newState != null)
        {
            movementType.Exit();
            movementType = newState;
            movementType.Enter();
        }
    }

    public void SlamPlayer2(InputAction.CallbackContext context)
    {
        MovementTypeState newState = movementType.SlamPlayer2(context);
        if (newState != null)
        {
            movementType.Exit();
            movementType = newState;
            movementType.Enter();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Enemy"))
        {
            if(isSlamming)
            {
                isSlamming = false;
                Destroy(collision.gameObject);
                playerRigidBody.velocity = Vector3.zero;
                player1Jumped = false;
                player2Jumped = false;
                movementType = movementType.BounceOff();
                movementType.Enter();
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
        if (collision.gameObject.tag.Equals("JumpPad"))
        {
            if (isSlamming)
            {
                isSlamming = false;
                playerRigidBody.velocity = Vector3.zero;
                player1Jumped = false;
                player2Jumped = false;
                movementType = movementType.BounceOff();
                movementType.Enter();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
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
    }
    public void RightDash()
    {
        isDashingRight = true;
        player1DashTime = Time.time;
        dashingTimeElapsed = 0f;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.gravityScale = 0f;
    }
    public void LeftDashArrow()
    {
        isDashingLeft = true;
        player2DashTime = Time.time;
        dashingTimeElapsed = 0f;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.gravityScale = 0f;
    }
    public void RightDashArrow()
    {
        isDashingRight = true;
        player2DashTime = Time.time;
        dashingTimeElapsed = 0f;
        playerRigidBody.velocity = Vector3.zero;
        playerRigidBody.gravityScale = 0f;
    }
}
