using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Enemigos")]
    [Tooltip("Total de enemigos en la escena")]
    public int totalEnemies;
    private int enemiesKilled = 0;

    [Header("Canvas")]
    public GameObject gameOverCanvas;
    public GameObject victoryCanvas;

    [Header("Victory UI")]
    public TextMeshProUGUI scoreText;

    [Header("Escenas")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "Game";

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Contar enemigos automáticamente si no se asigna manualmente
        if (totalEnemies == 0)
            totalEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None).Length;

        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        if (victoryCanvas != null) victoryCanvas.SetActive(false);

        // Desbloquear cursor por si acaso
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Llamado desde EnemyHealth cuando muere un enemigo ──
    public void EnemyKilled()
    {
        if (gameEnded) return;

        enemiesKilled++;

        if (enemiesKilled >= totalEnemies)
            ShowVictory();
    }

    // Llamado desde PlayerHealth cuando muere el jugador ──
    public void ShowGameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);

        // Mostrar cursor para los botones
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; // pausar el juego
    }

    private void ShowVictory()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (victoryCanvas != null) victoryCanvas.SetActive(true);

        if (scoreText != null)
            scoreText.text = "Enemigos eliminados: " + enemiesKilled + "/" + totalEnemies;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; // pausar el juego

    }

    // ── Botones ──
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
