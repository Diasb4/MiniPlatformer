using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Игрок словил люлей");

            // Отключаем управление игроком
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            else
            {
                Debug.LogWarning("PlayerMovement не найден на объекте игрока!");
            }

            // Вызываем метод UIManager для показа GameOverPanel
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowGameOver();
            }
            else
            {
                Debug.LogWarning("UIManager не найден!");
            }
        }
    }
}