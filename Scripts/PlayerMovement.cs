using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // Существующие параметры движения
    public float moveSpeed = 5f;
    public float jumpForce = 14f;
    private Rigidbody2D rb;
    private bool isGrounded;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    [SerializeField] private Animator animator;
    [SerializeField] private float fallThresholdY = -10f;

    // Параметры камней
    [Header("Камни")]
    public GameObject stonePrefab; // Префаб камня, который будет бросаться
    public Transform throwPoint; // Точка, откуда будет вылетать камень
    public float throwForce = 10f; // Сила броска
    public float throwCooldown = 0.5f; // Время между бросками
    private bool canThrow = true;
    private int stonesCount = 0; // Счетчик подобранных камней
    [SerializeField] private LayerMask stoneLayer; // Слой для камней
    [SerializeField] private float pickupRadius = 1f; // Радиус для подбора камней

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D не найден на объекте!");
        }
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck Transform не назначен!");
        }
        if (animator == null)
        {
            Debug.LogError("Animator не назначен!");
        }
        if (throwPoint == null)
        {
            Debug.LogError("ThrowPoint Transform не назначен!");
        }
        if (stonePrefab == null)
        {
            Debug.LogError("StonePrefab не назначен!");
        }
    }

    void Update()
    {
        // Существующий код движения
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        float speed = Mathf.Abs(moveInput);
        animator.SetFloat("Speed", speed);

        // Проверка падения
        if (transform.position.y < fallThresholdY)
        {
            FindObjectOfType<UIManager>().ShowGameOver();
            enabled = false;
        }

        // Новый код для подбора камней
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupStone();
        }

        // Новый код для бросания камней
        if (Input.GetMouseButtonDown(0) && canThrow && stonesCount > 0)
        {
            ThrowStone();
        }
    }

    private void TryPickupStone()
    {
        // Поиск камней в области вокруг игрока
        Collider2D[] stoneColliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius, stoneLayer);
        if (stoneColliders.Length > 0)
        {
            // Берем первый найденный камень
            GameObject stone = stoneColliders[0].gameObject;
            // Увеличиваем счетчик камней
            stonesCount++;
            // Показываем информацию на UI
            UpdateStonesUI();
            // Удаляем поднятый камень со сцены
            Destroy(stone);
            // Можно добавить звук или эффект подбора камня
            Debug.Log("Подобран камень! Теперь у вас " + stonesCount + " камней.");
        }
    }

    private void ThrowStone()
    {
        // Создаем камень в точке броска
        GameObject stone = Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D stoneRb = stone.GetComponent<Rigidbody2D>();

        if (stoneRb != null)
        {
            // Направление броска зависит от направления персонажа
            Vector2 throwDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // Получаем направление мыши от точки броска
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)throwPoint.position).normalized;

            // Применяем силу к камню
            stoneRb.linearVelocity = direction * throwForce;

            // Уменьшаем счетчик камней
            stonesCount--;

            // Обновляем UI
            UpdateStonesUI();

            // Запускаем кулдаун
            StartCoroutine(ThrowCooldown());
        }
    }

    private IEnumerator ThrowCooldown()
    {
        canThrow = false;
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }

    private void UpdateStonesUI()
    {
        // Здесь обновляем UI с количеством камней
        // Пример: GetComponent<UIManager>().UpdateStonesCount(stonesCount);
        // Замените на вашу реализацию UI
        Debug.Log("Количество камней: " + stonesCount);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            FindObjectOfType<UIManager>().ShowGameOver();
            enabled = false;
        }
    }

    // Визуализация радиуса подбора в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);

        if (throwPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(throwPoint.position, 0.2f);
        }
    }
}