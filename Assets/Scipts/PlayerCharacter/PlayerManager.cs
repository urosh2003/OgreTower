using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public Transform playerTransform;
    [SerializeField] public float walkingSpeed;
    [SerializeField] public float runningSpeed;
    public Rigidbody2D playerRigidBody;

    public bool isGrounded;
    public bool player1Jumped;
    public bool player2Jumped;
    [SerializeField] public float jumpForce;

    public bool isSlamming;

    [SerializeField] public float slamForce;
    [SerializeField] public float dashSpeed;

    [SerializeField] public LayerMask groundLayerMask;
    [SerializeField] public Vector2 groundedBoxCast;

    public bool isDashing;

    [SerializeField] public float DASH_DURATION_CONST = 0.2f;

    public float gravityScale;
    public float fallingGravity;

    public Animator animator;
    public static event Action movementDirectionChanged;
    public static event Action jumped;
    public static event Action<float> hasGrounded;
    public float lastMovementDirection  = -1;

    private void Awake()
    {
        Instance = this;
    }

    public MovementDirectionState movementDirection;
    public MovementTypeState movementType;

    // Start is called before the first frame update
    void Start()
    {
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
        movementDirection.Update();

        MovementTypeState newState = movementType.Update();
        if (newState != null)
        {
            movementType.Exit();
            movementType = newState;
            movementType.Enter();
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

    public void JumpPlayer1(InputAction.CallbackContext context)
    {
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
        MovementTypeState newState = movementType.JumpPlayer2(context);
        if (newState != null)
        {
            movementType.Exit();
            movementType = newState;
            movementType.Enter();
            jumped.Invoke();
        }
    }

    public void BounceOff()
    {
        MovementTypeState newState = movementType.BounceOff();
        if (newState != null)
        {
            movementType.Exit();
            movementType = newState;
            movementType.Enter();
            jumped.Invoke();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Enemy"))
        {
            if(isSlamming)
            {
                Destroy(collision.gameObject);
                BounceOff();
            }
            else if(isDashing)
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
                BounceOff();
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
}
