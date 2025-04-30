using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject blackBackground;
    [SerializeField] private Image gameOverImage;
    [SerializeField] private Button restartButton;

    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float fallThreshold = -10f;

    [Header("Enemy Settings")]
    [SerializeField] private Collider2D playerCollider;

    private bool isGameOver = false;

    private void Awake()
    {
        ValidateUIElements();
    }

    private void Update()
{
    if (!isGameOver)
    {
        if (IsPlayerFallen())
        {
            ShowGameOver();
        }
        else if (IsPlayerCollidingWithEnemy())
        {
            ShowGameOver();
        }
    }
}

    private void ValidateUIElements()
    {
        if (gameOverPanel == null || restartButton == null || blackBackground == null || gameOverImage == null)
        {
            Debug.LogError("Не все UI элементы назначены в инспекторе!");
        }
        else
        {
            InitializeGameOverUI();
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform не назначен в инспекторе!");
        }

        if (playerCollider == null)
        {
            Debug.LogError("Player Collider не назначен в инспекторе!");
        }
    }

    private void InitializeGameOverUI()
    {
        gameOverPanel.SetActive(false);
        blackBackground.SetActive(false);
        gameOverImage.enabled = false;
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartGame);
    }

    private bool IsPlayerFallen()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform не назначен или был уничтожен!");
            return false;
        }

        return playerTransform.position.y < fallThreshold;
    }


    private bool IsPlayerCollidingWithEnemy()
    {
        if (playerCollider == null) return false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(playerTransform.position, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel == null || blackBackground == null || gameOverImage == null) return;

        isGameOver = true;
        blackBackground.SetActive(true);
        gameOverPanel.SetActive(true);
        gameOverImage.enabled = true;

        Time.timeScale = 0f;
    }

    public void HideGameOver()
    {
        if (gameOverPanel == null || blackBackground == null || gameOverImage == null) return;

        isGameOver = false;
        gameOverPanel.SetActive(false);
        blackBackground.SetActive(false);
        gameOverImage.enabled = false;

        Time.timeScale = 1f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
