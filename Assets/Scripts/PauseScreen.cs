using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    // Reference to the pause panel
    public GameObject pausePanel;

    // Is the game currently paused?
    public bool isPaused = false;

    public bool justResumed = false;

    void Update()
    {
        // Toggle pause on Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // Show pause panel
        pausePanel.SetActive(true);
        // Stop the game
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        justResumed = true;
    }

    public void RestartGame()
    {
        // Reset time scale before restarting
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}