using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Здесь можно сделать счетчик монет
            Destroy(gameObject);
        }
    }
}
