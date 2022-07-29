using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletSpawnPoint;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D feetCollider;
    private bool isAlive = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if(isAlive == false)
        {
            return; 
        }

        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    private void OnFire(InputValue value)
    {
        if (isAlive == false)
        {
            return;
        }

        Instantiate(bullet, bulletSpawnPoint.transform.position, Quaternion.identity);
    }

    private void Run()
    {
        if (isAlive == false)
        {
            return;
        }

        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        bool isPlayerRunning = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", isPlayerRunning);
    }

    private void FlipSprite()
    {
        bool isPlayerRunning = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        if (isPlayerRunning)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }
    }

    private void OnMove(InputValue value)
    {
        if (isAlive == false)
        {
            return;
        }

        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (isAlive == false)
        {
            return;
        }

        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && value.isPressed)
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private void ClimbLadder()
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
            rb.velocity = climbVelocity;
            rb.gravityScale = 0;

            bool isPlayerClimbingLadder = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
            animator.SetBool("isClimbing", isPlayerClimbingLadder);
        }
        else if (!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb.gravityScale = 6;
            animator.SetBool("isClimbing", false);
        }
    }

    private void Die()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            animator.SetTrigger("dying");
            rb.velocity = deathKick;
        }
    }
}
