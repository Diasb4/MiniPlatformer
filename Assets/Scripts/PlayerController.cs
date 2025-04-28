using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 14f;
    private Rigidbody2D rb;
    private bool isGrounded;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    // Синглтон
    public static PlayerController Instance { get; private set; }

    // Позиция последнего чекпоинта
    private Vector2 lastCheckpointPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Устанавливаем начальную позицию чекпоинта (например, начальная позиция игрока)
        lastCheckpointPosition = transform.position;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // Метод для установки чекпоинта
    public void SetCheckpoint(Vector2 position)
    {
        lastCheckpointPosition = position;
        Debug.Log($"Чекпоинт установлен: {lastCheckpointPosition}");
    }

    // Метод для перемещения игрока в последний чекпоинт
    public void RespawnAtCheckpoint()
    {
        transform.position = lastCheckpointPosition;
        rb.linearVelocity = Vector2.zero; // Сбрасываем скорость
        Debug.Log($"Игрок возрожден в чекпоинте: {lastCheckpointPosition}");
    }
}