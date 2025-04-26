using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask platformLayer;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private MovingPlatform currentPlatform;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
     
    void FixedUpdate()
    {
        UpdatePlatformVelocity();
        HandleMovement();
        HandleJump();
    }

    void UpdatePlatformVelocity()
    {
        Collider2D col = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);
        if (col != null)
        {
            var mp = col.GetComponent<MovingPlatform>();
            if (mp != null)
            {
                if (currentPlatform != mp)
                {
                    currentPlatform = mp;
                }

                return;
            }
        }

        if (currentPlatform != null)
        {
            currentPlatform = null;
        }
    }
    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector2 vel = rb.linearVelocity;

        float vx = moveInput * moveSpeed;

        if (currentPlatform != null)
            vx += currentPlatform.Velocity.x;
        
        vel.x = vx;

        if (Mathf.Abs(vel.y) < 0.05f) vel.y = 0;
        rb.linearVelocity = vel;
    }

    void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, 
            groundCheckRadius, 
            groundLayer
        );
        
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    //Visualization of isGrounded
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
