using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Player Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float returnDelay = 3f;

    private bool isChasing = false;
    private bool isReturning = false;
    private float returnTimer;
    private Transform player;
    private Animator animator;
    private EnemyMovement movement;

    void Start()
    {
        if (PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
            Debug.Log("Игрок найден через PlayerController.Instance!");
        }
        else
        {
            Debug.LogWarning("Игрок не найден! Убедитесь, что PlayerController существует в сцене.");
        }

        animator = GetComponent<Animator>();
        movement = GetComponent<EnemyMovement>();

        if (movement == null)
        {
            Debug.LogError("EnemyMovement не найден на объекте!");
            enabled = false;
        }
    }

    void Update()
    {
        if (player == null && PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
            Debug.Log("Игрок найден в Update через PlayerController.Instance!");
        }

        Debug.Log($"isChasing: {isChasing}, isReturning: {isReturning}, returnTimer: {returnTimer}");

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
            movement.Patrol();
        }
    }

    void CheckForPlayer()
    {
        if (player == null)
        {
            Debug.Log("Player is null! Не могу обнаружить игрока.");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Debug.Log($"Расстояние до игрока: {distanceToPlayer}, Радиус обнаружения: {detectionRange}");

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            Debug.Log($"Направление к игроку: {directionToPlayer}");

            // Raycast для обнаружения игрока
            RaycastHit2D playerCheck = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, playerLayer);
            bool playerDetected = playerCheck.collider != null && playerCheck.collider.gameObject.CompareTag("Player");
            Debug.Log($"Игрок обнаружен: {playerDetected}, Объект: {(playerCheck.collider != null ? playerCheck.collider.name : "null")}");

            // Визуализация Raycast в редакторе
            Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, playerDetected ? Color.green : Color.red, 0.1f);

            if (playerDetected)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    isReturning = false;
                    returnTimer = 0f;
                    Debug.Log("Начинаю преследовать игрока!");
                }
            }
            else
            {
                StartReturnTimer();
                Debug.Log("Игрок не обнаружен Raycast'ом, начинаю таймер возврата.");
            }
        }
        else
        {
            StartReturnTimer();
            Debug.Log("Игрок слишком далеко, начинаю таймер возврата.");
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
            Debug.Log("Преследование остановлено, начинаю таймер возврата.");
        }

        if (!isReturning && returnTimer > 0)
        {
            returnTimer -= Time.deltaTime;
            if (returnTimer <= 0)
            {
                isReturning = true;
                Debug.Log("Таймер возврата истек, возвращаюсь на начальную позицию.");
            }
        }
    }

    void ChasePlayer()
    {
        if (player == null)
        {
            Debug.Log("Player is null в ChasePlayer!");
            return;
        }

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        transform.Translate(Vector2.right * direction * chaseSpeed * Time.deltaTime);
        Debug.Log($"Преследую игрока, направление: {direction}, скорость: {chaseSpeed}");

        bool movingRight = movement.IsMovingRight();
        if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
        {
            movement.SetMovingRight(!movingRight);
            transform.localScale = new Vector3(movingRight ? 1f : -1f, 1f, 1f);
            Debug.Log($"Поворачиваю врага, movingRight: {movingRight}");
        }
    }

    void ReturnToStart()
    {
        Vector3 startPosition = movement.GetStartPosition();
        float distanceToStart = Mathf.Abs(transform.position.x - startPosition.x);
        if (distanceToStart <= 0.1f)
        {
            isReturning = false;
            Debug.Log("Достиг начальной позиции, прекращаю возврат.");
            return;
        }

        float direction = startPosition.x > transform.position.x ? 1f : -1f;
        transform.Translate(Vector2.right * direction * chaseSpeed * Time.deltaTime);
        Debug.Log($"Возвращаюсь на начальную позицию, направление: {direction}");

        bool movingRight = movement.IsMovingRight();
        if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
        {
            movement.SetMovingRight(!movingRight);
            transform.localScale = new Vector3(movingRight ? 1f : -1f, 1f, 1f);
            Debug.Log($"Поворачиваю врага при возврате, movingRight: {movingRight}");
        }
    }
}