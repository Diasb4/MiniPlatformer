using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("����� ������ �����");
            // �������� ����� UIManager ��� ������ GameOverPanel
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver();
            }
            else
            {
                Debug.LogWarning("UIManager �� ������!");
            }
        }
    }
}