using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    private void Run()
    {
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
        moveInput = value.Get<Vector2>();
        //Debug.Log(moveInput);
    }

    private void OnJump(InputValue value)
    {
        if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && value.isPressed)
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private void ClimbLadder()
    {
        if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
            rb.velocity = climbVelocity;
            rb.gravityScale = 0;

            bool isPlayerClimbingLadder = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
            animator.SetBool("isClimbing", isPlayerClimbingLadder);
        }
        else if (!capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb.gravityScale = 8;
            animator.SetBool("isClimbing", false);
        }
    }
}
