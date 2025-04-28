using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float checkDistance = 0.3f;
    [SerializeField] private Transform groundDetection;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float wanderRadius = 5f;

    [Header("Wall Check Settings")]
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private Transform wallCheck;

    private bool movingRight = true;
    private Vector3 startPosition;
    private float directionChangeCooldown = 0.5f;
    private float lastDirectionChangeTime;

    void Start()
    {
        startPosition = transform.position;

        if (groundDetection == null || wallCheck == null)
        {
            Debug.LogError("GroundDetection или WallCheck не назначены!");
            enabled = false;
        }
    }

    public void Patrol()
    {
        if (Time.time - lastDirectionChangeTime < directionChangeCooldown)
            return;

        float distanceFromStartX = transform.position.x - startPosition.x;
        Debug.Log($"Позиция X: {transform.position.x}, Начало X: {startPosition.x}, Расстояние: {distanceFromStartX}, Радиус: {wanderRadius}");
        if ((distanceFromStartX >= wanderRadius && movingRight) || (distanceFromStartX <= -wanderRadius && !movingRight))
        {
            movingRight = !movingRight;
            lastDirectionChangeTime = Time.time;
            Debug.Log($"Достиг радиуса, movingRight: {movingRight}");
            return;
        }

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

        float moveDirection = movingRight ? 1f : -1f;
        Vector2 movement = Vector2.right * moveDirection * speed * Time.deltaTime;
        Debug.Log($"Движение: {movement}, Позиция: {transform.position}");
        transform.Translate(movement);

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
    }

    public bool IsMovingRight()
    {
        return movingRight;
    }

    public void SetMovingRight(bool value)
    {
        movingRight = value;
    }

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }
}