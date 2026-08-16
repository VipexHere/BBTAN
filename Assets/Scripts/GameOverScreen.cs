using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    // Reference to the Game Over panel
    public GameObject gameOverPanel;

    // Reference to the high score text
    public TextMeshProUGUI highScoreText;

    // Key for saving high score in PlayerPrefs
    private const string HIGH_SCORE_KEY = "HighScore";

    // Reference to the high score display on main screen
    public TextMeshProUGUI highScoreDisplay;

    void Start()
    {
        // Show current high score on main screen at game start
        int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        highScoreDisplay.text = "TOP " + highScore.ToString();
    }

    // Show the Game Over screen and update high score
    public void ShowGameOver(int currentScore)
    {
        // Get current high score
        int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        // Update high score if current score is better
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        // Show high score on screen
        highScoreText.text = "Best: " + highScore.ToString();

        // Update high score display on main screen
        highScoreDisplay.text = "TOP " + highScore.ToString();

        gameOverPanel.SetActive(true);
    }

    // Restart the game by reloading the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}