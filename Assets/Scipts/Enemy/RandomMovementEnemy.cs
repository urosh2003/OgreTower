using UnityEngine;

public class RandomMovementEnemy : MonoBehaviour
{
    private Rigidbody2D enemyRigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float movementSpeed;


    [SerializeField] private float moveDurationLow;
    [SerializeField] private float moveDurationHigh;
    [SerializeField] private float moveDuration;

    [SerializeField] private float waitDurationLow;
    [SerializeField] private float waitDurationHigh;
    [SerializeField] private float waitDuration;

    [SerializeField] private float timeElapsed;
    [SerializeField] private bool isMoving;

    private Vector2 direction;

    [SerializeField] private bool staticEnemy;

    // Start is called before the first frame update
    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        direction =  new Vector2(-1, 0); 
    }

    // Update is called once per frame
    void Update()
    {
        if (staticEnemy)
        {
            enemyRigidBody.velocity = Vector2.zero;
            return;
        }
        timeElapsed += Time.deltaTime;
        if (isMoving)
        {
            if(timeElapsed >= moveDuration) 
            {
                timeElapsed = 0;
                isMoving = false;
                animator.SetBool("isMoving", false);
                enemyRigidBody.velocity = Vector2.zero;
                waitDuration = Random.Range(waitDurationLow, waitDurationHigh);
            }
        }
        else if(timeElapsed >= waitDuration)
        {
            timeElapsed = 0;
            isMoving = true;
            animator.SetBool("isMoving", true);
            moveDuration = Random.Range(moveDurationLow, moveDurationHigh);
            direction *= -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            enemyRigidBody.velocity = direction * movementSpeed;
        }
        
    }
}
