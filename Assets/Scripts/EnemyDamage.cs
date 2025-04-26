using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // —юда пиши: убить игрока, откатить сцену и т.д.
            Debug.Log("»грок словил люлей");
        }
    }
}
