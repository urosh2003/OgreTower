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
    public bool introLevel;
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
    public static event Action slamLanded;
    public static event Action dashHit;
    public static event Action<float> hasGrounded;
    public static event Action<Vector2> dashStarted;
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
        animator.SetBool("left", true);
        movementDirection = new IdleState();
        movementDirection.Enter();
        movementType = new GroundedState();
        movementDirection.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        if(lastMovementDirection == -1)
            GetComponentInChildren<SpriteRenderer>().flipX = false;
        else
            GetComponentInChildren<SpriteRenderer>().flipX = true;

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
        if (introLevel && !isGrounded)
            return;

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
        if (introLevel && !isGrounded)
            return;

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
        SlamLanded();
        StartCoroutine(StopGame());
        MovementTypeState newState = movementType.BounceOff();
        if (newState != null)
        {
            movementType.Exit();
            movementType = newState;
            movementType.Enter();
            jumped.Invoke();
        }
    }

    public void SlamLanded()
    {
        slamLanded?.Invoke();
    }

    public void Dashing(float direction)
    {
        dashStarted?.Invoke(new Vector2(direction, 0));
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
                dashHit?.Invoke();
                StartCoroutine(StopGame());
                Destroy(collision.gameObject);
            }
            else
            {
                CheckpointManager.instance.PlayerDied();
            }
        }
        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            if (isSlamming)
            {
                BounceOff();
                Destroy(collision.gameObject);
            }
            else if (isDashing)
            {
                dashHit?.Invoke();
                StartCoroutine(StopGame());
                Destroy(collision.gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag.Equals("Spike"))
        {
            CheckpointManager.instance.PlayerDied();
        }
        else if (collision.gameObject.tag.Equals("Checkpoint"))
        {
            CheckpointManager.instance.SetChekpoint(collision.transform);
        }
        else if (collision.gameObject.tag.Equals("LevelEnd"))
        {
            CheckpointManager.instance.NextLevel();
        }
        else if (collision.gameObject.tag.Equals("JumpPad"))
        {
            BounceOff();
        }
    }

    public float hitStopDuration;

    IEnumerator StopGame()
    {
        // Stop the game for a set duration
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = originalTimeScale;
    }
}
