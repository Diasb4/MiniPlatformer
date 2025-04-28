using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    [SerializeField] private Vector2 spawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                SceneManager.sceneLoaded += (scene, mode) => OnSceneLoaded(scene, mode, player);
                SceneManager.LoadScene(nextLevelName);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode, PlayerController player)
    {
        player.transform.position = spawnPosition;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        SceneManager.sceneLoaded -= (scene, mode) => OnSceneLoaded(scene, mode, player);
    }
}