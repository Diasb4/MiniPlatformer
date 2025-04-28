using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Игрок словил люлей");
            // Вызываем метод UIManager для показа GameOverPanel
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver();
            }
            else
            {
                Debug.LogWarning("UIManager не найден!");
            }
        }
    }
}