using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 14f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.2f;  // Радиус проверки земли
    [SerializeField] private Transform groundCheck;          // Трансформ для проверки земли
    [SerializeField] private LayerMask groundLayer;          // Слой земли

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck Transform не назначен!");
        }
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        Move();
        CheckGround();
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        canJump = false;
        Debug.Log("Прыжок выполнен!");
    }

    void CheckGround()
    {
        Vector2 position = groundCheck.position;
        isGrounded = Physics2D.OverlapCircle(position, groundCheckRadius, groundLayer);
        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
        Debug.Log($"isGrounded: {isGrounded}, canJump: {canJump}, VelocityY: {rb.linearVelocity.y}");
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}