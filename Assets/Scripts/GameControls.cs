using UnityEngine;

public class GameControls : MonoBehaviour
{
    // Is the game currently sped up?
    private bool isSpedUp = false;

    // Normal and fast time scales
    private float normalTimeScale = 1f;
    private float fastTimeScale = 3f;

    // Reference to the player
    private Player player;

    // References to the buttons
    public GameObject speedUpButton;
    public GameObject recallButton;

    void Start()
    {
        player = FindObjectOfType<Player>();
        // Hide buttons at start
        speedUpButton.SetActive(false);
        recallButton.SetActive(false);
    }

    // Show buttons during ball flight
    public void ShowButtons()
    {
        speedUpButton.SetActive(true);
        recallButton.SetActive(true);
    }

    // Hide buttons when balls have landed
    public void HideButtons()
    {
        speedUpButton.SetActive(false);
        recallButton.SetActive(false);
    }

    // Toggle speed up
    public void ToggleSpeedUp()
    {
        if (isSpedUp)
        {
            Time.timeScale = normalTimeScale;
            isSpedUp = false;
        }
        else
        {
            Time.timeScale = fastTimeScale;
            isSpedUp = true;
        }
    }

    // Recall all balls
    public void RecallBalls()
    {
        // Stop shooting coroutine first
        player.StopShooting();

        // Set ballsInFlight to actual number of balls in scene
        Ball[] allBalls = FindObjectsOfType<Ball>();
        player.ballsInFlight = allBalls.Length;

        // Destroy all balls and inform player they landed
        foreach (Ball ball in allBalls)
        {
            player.OnBallLanded(player.transform.position);
            Destroy(ball.gameObject);
        }
    }

    // Reset speed up state
    public void ResetSpeedUp()
    {
        isSpedUp = false;
    }
}