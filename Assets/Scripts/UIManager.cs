using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;

    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel == null || restartButton == null)
        {
            Debug.LogError("GameOverPanel ��� RestartButton �� ��������� � ����������!");
        }
        else
        {
            gameOverPanel.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void RestartGame()
    {
        // �������� GameOverPanel
        HideGameOver();

        // ���������� ������ � ��������� ���������
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.RespawnAtCheckpoint();
        }
        else
        {
            Debug.LogWarning("PlayerController �� ������ ��� ��������!");
        }
    }
}