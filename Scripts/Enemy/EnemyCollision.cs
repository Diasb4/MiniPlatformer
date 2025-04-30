using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("����� ������ �����");

            // ��������� ���������� �������
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            else
            {
                Debug.LogWarning("PlayerMovement �� ������ �� ������� ������!");
            }

            // �������� ����� UIManager ��� ������ GameOverPanel
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowGameOver();
            }
            else
            {
                Debug.LogWarning("UIManager �� ������!");
            }
        }
    }
}