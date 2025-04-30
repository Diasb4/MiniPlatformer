using UnityEngine;

public class Stone : MonoBehaviour
{
    public float lifetime = 5f; // ����� ����� ����� ����� ������
    public int damage = 1; // ���� �� �����
    public GameObject hitEffect; // ������ ��� ��������� (�����������)

    private void Start()
    {
        // ���������� ������ ����� ������������ �����
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���� ������ ����� �� �����
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // ���������, ���� �� � ����� ��������� ��������
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // ������� ������ ���������, ���� �� ��������
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // ���������� ������
            Destroy(gameObject);
        }
        // ���� ������ ����� � ����� ��� ������ �������
        else if (!collision.gameObject.CompareTag("Player"))
        {
            // ����� �������� ���� ����� � �����������

            // ���������� ������ ����� �������� ����� ����� ����� � �����������
            // ��� ����� �������� ��� �� ��������� ����� � ��������� ��������� �����
            Destroy(gameObject, 2f);
        }
    }
}