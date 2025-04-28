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
            Debug.Log("����� ������ ����� PlayerController.Instance!");
        }
        else
        {
            Debug.LogWarning("����� �� ������! ���������, ��� PlayerController ���������� � �����.");
        }

        animator = GetComponent<Animator>();
        movement = GetComponent<EnemyMovement>();

        if (movement == null)
        {
            Debug.LogError("EnemyMovement �� ������ �� �������!");
            enabled = false;
        }
    }

    void Update()
    {
        if (player == null && PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
            Debug.Log("����� ������ � Update ����� PlayerController.Instance!");
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
            Debug.Log("Player is null! �� ���� ���������� ������.");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Debug.Log($"���������� �� ������: {distanceToPlayer}, ������ �����������: {detectionRange}");

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            Debug.Log($"����������� � ������: {directionToPlayer}");

            // Raycast ��� ����������� ������
            RaycastHit2D playerCheck = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, playerLayer);
            bool playerDetected = playerCheck.collider != null && playerCheck.collider.gameObject.CompareTag("Player");
            Debug.Log($"����� ���������: {playerDetected}, ������: {(playerCheck.collider != null ? playerCheck.collider.name : "null")}");

            // ������������ Raycast � ���������
            Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, playerDetected ? Color.green : Color.red, 0.1f);

            if (playerDetected)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    isReturning = false;
                    returnTimer = 0f;
                    Debug.Log("������� ������������ ������!");
                }
            }
            else
            {
                StartReturnTimer();
                Debug.Log("����� �� ��������� Raycast'��, ������� ������ ��������.");
            }
        }
        else
        {
            StartReturnTimer();
            Debug.Log("����� ������� ������, ������� ������ ��������.");
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
            Debug.Log("������������� �����������, ������� ������ ��������.");
        }

        if (!isReturning && returnTimer > 0)
        {
            returnTimer -= Time.deltaTime;
            if (returnTimer <= 0)
            {
                isReturning = true;
                Debug.Log("������ �������� �����, ����������� �� ��������� �������.");
            }
        }
    }

    void ChasePlayer()
    {
        if (player == null)
        {
            Debug.Log("Player is null � ChasePlayer!");
            return;
        }

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        transform.Translate(Vector2.right * direction * chaseSpeed * Time.deltaTime);
        Debug.Log($"��������� ������, �����������: {direction}, ��������: {chaseSpeed}");

        bool movingRight = movement.IsMovingRight();
        if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
        {
            movement.SetMovingRight(!movingRight);
            transform.localScale = new Vector3(movingRight ? 1f : -1f, 1f, 1f);
            Debug.Log($"����������� �����, movingRight: {movingRight}");
        }
    }

    void ReturnToStart()
    {
        Vector3 startPosition = movement.GetStartPosition();
        float distanceToStart = Mathf.Abs(transform.position.x - startPosition.x);
        if (distanceToStart <= 0.1f)
        {
            isReturning = false;
            Debug.Log("������ ��������� �������, ��������� �������.");
            return;
        }

        float direction = startPosition.x > transform.position.x ? 1f : -1f;
        transform.Translate(Vector2.right * direction * chaseSpeed * Time.deltaTime);
        Debug.Log($"����������� �� ��������� �������, �����������: {direction}");

        bool movingRight = movement.IsMovingRight();
        if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
        {
            movement.SetMovingRight(!movingRight);
            transform.localScale = new Vector3(movingRight ? 1f : -1f, 1f, 1f);
            Debug.Log($"����������� ����� ��� ��������, movingRight: {movingRight}");
        }
    }
}