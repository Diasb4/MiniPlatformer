using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // ���� ����: ����� ������, �������� ����� � �.�.
            Debug.Log("����� ������ �����");
        }
    }
}
