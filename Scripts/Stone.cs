using UnityEngine;

public class Stone : MonoBehaviour
{
    public float lifetime = 5f; // Время жизни камня после броска
    public int damage = 1; // Урон от камня
    public GameObject hitEffect; // Эффект при попадании (опционально)

    private void Start()
    {
        // Уничтожаем камень через определенное время
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если камень попал во врага
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Проверяем, есть ли у врага компонент здоровья
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Создаем эффект попадания, если он назначен
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // Уничтожаем камень
            Destroy(gameObject);
        }
        // Если камень попал в землю или другие объекты
        else if (!collision.gameObject.CompareTag("Player"))
        {
            // Можно добавить звук удара о поверхность

            // Уничтожаем камень через короткое время после удара о поверхность
            // или можно оставить его на некоторое время и позволить подобрать снова
            Destroy(gameObject, 2f);
        }
    }
}