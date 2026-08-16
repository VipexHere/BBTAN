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

    void Start()
    {
        player = FindObjectOfType<Player>();
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
        // Find and destroy all balls in the scene
        Ball[] allBalls = FindObjectsOfType<Ball>();
        foreach (Ball ball in allBalls)
        {
            // Inform player that ball landed at player's position
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