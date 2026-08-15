using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    // Reference to the Game Over panel
    public GameObject gameOverPanel;

    // Show the Game Over screen
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    // Restart the game by reloading the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}