using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float checkDistance = 0.3f;
    [SerializeField] private Transform groundDetection;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float pauseTimeAtEdge = 2f;

    [Header("Wall Check Settings")]
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private Transform wallCheck;

    [Header("Player Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float returnDelay = 3f;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;

    private bool movingRight = true;
    private bool isChasing = false;
    private bool isPaused = false;
    private bool isReturning = false;
    private float returnTimer;
    private Transform player;
    private Animator animator;
    private Vector3 startPosition;
    private float pauseTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("Игрок не найден! Убедитесь, что у игрока есть тег 'Player'.");
        }

        animator = GetComponent<Animator>();
        if (groundDetection == null || wallCheck == null)
        {
            Debug.LogError("GroundDetection или WallCheck не назначены!");
            return;
        }

        if (gameOverPanel == null || restartButton == null)
        {
            Debug.LogError("GameOverPanel или RestartButton не назначены в инспекторе!");
        }
        else
        {
            gameOverPanel.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }

        startPosition = transform.position;
        pauseTimer = 0f;
        returnTimer = 0f;
    }

    void Update()
    {
        Debug.Log($"isChasing: {isChasing}, isReturning: {isReturning}, isPaused: {isPaused}, returnTimer: {returnTimer}");

        CheckForPlayer();

        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else if (isReturning)
        {
            ReturnToStart();
        }
        else
        {
            Wander();
        }
    }

    private float directionChangeCooldown = 0.5f;
    private float lastDirectionChangeTime;

    void Wander()
    {
        // Пропускаем, если смена направления на кулдауне
        if (Time.time - lastDirectionChangeTime < directionChangeCooldown)
            return;

        // Проверяем расстояние от начальной точки
        float distanceFromStartX = transform.position.x - startPosition.x;
        Debug.Log($"Позиция X: {transform.position.x}, Начало X: {startPosition.x}, Расстояние: {distanceFromStartX}, Радиус: {wanderRadius}");
        if ((distanceFromStartX >= wanderRadius && movingRight) || (distanceFromStartX <= -wanderRadius && !movingRight))
        {
            movingRight = !movingRight;
            lastDirectionChangeTime = Time.time;
            Debug.Log($"Достиг радиуса, movingRight: {movingRight}");
            return;
        }

        // Проверка земли и стены
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, checkDistance, groundLayer);
        RaycastHit2D wallInfo = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, groundLayer);
        Debug.Log($"GroundInfo: {(groundInfo.collider != null ? groundInfo.collider.name : "null")}, WallInfo: {(wallInfo.collider != null ? wallInfo.collider.name : "null")}");

        if (groundInfo.collider == null || wallInfo.collider != null)
        {
            movingRight = !movingRight;
            lastDirectionChangeTime = Time.time;
            Debug.Log($"Край платформы или стена, movingRight: {movingRight}");
            return;
        }

        // Движение
        float moveDirection = movingRight ? 1f : -1f;
        Vector2 movement = Vector2.right * moveDirection * speed * Time.deltaTime;
        Debug.Log($"Движение: {movement}, Позиция: {transform.position}");
        transform.Translate(movement);

        // Обновляем масштаб
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
    }
    void Flip()
    {
        transform.localScale = new Vector3(movingRight ? 1f : -1f, 1f, 1f);
    }

    void CheckForPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            RaycastHit2D playerCheck = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, groundLayer | playerLayer);
            bool playerDetected = playerCheck.collider != null && playerCheck.collider.gameObject.CompareTag("Player");

            if (playerDetected)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    isReturning = false;
                    returnTimer = 0f;
                }
            }
            else
            {
                StartReturnTimer();
            }
        }
        else
        {
            StartReturnTimer();
        }

        if (animator != null)
        {
            animator.SetBool("isChasing", isChasing);
        }
    }

    void StartReturnTimer()
    {
        if (isChasing)
        {
            isChasing = false;
            returnTimer = returnDelay;
        }

        if (!isReturning && returnTimer > 0)
        {
            returnTimer -= Time.deltaTime;
            if (returnTimer <= 0)
            {
                isReturning = true;
            }
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        transform.Translate(Vector2.right * direction * chaseSpeed * Time.deltaTime);

        if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
        {
            movingRight = !movingRight;
            transform.localScale = new Vector3(movingRight ? 1f : -1f, 1f, 1f);
        }
    }

    void ReturnToStart()
    {
        float distanceToStart = Mathf.Abs(transform.position.x - startPosition.x);
        if (distanceToStart <= 0.1f)
        {
            isReturning = false;
            return;
        }

        float direction = startPosition.x > transform.position.x ? 1f : -1f;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
        {
            movingRight = !movingRight;
            transform.localScale = new Vector3(movingRight ? 1f : -1f, 1f, 1f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            Time.timeScale = 0f;
        }
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}