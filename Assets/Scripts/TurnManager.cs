using UnityEngine;
using System.Collections;
using TMPro;

public class TurnManager : MonoBehaviour
{
    // Referencje do innych skryptów
    private GridManager gridManager;
    private Player player;

    // Numer aktualnej tury
    private int currentTurn = 0;

    // Prevents EndTurnSequence from running multiple times
    private bool isEndingTurn = false;

    // Reference to the turn counter text
    public TextMeshProUGUI turnCounterText;

    // Reference to the Game Over screen
    private GameOverScreen gameOverScreen;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        player = FindObjectOfType<Player>();
        StartTurn();
        gameOverScreen = FindObjectOfType<GameOverScreen>();
    }

    public void OnShootingFinished()
    {
        // Prevent multiple calls from triggering multiple turn endings
        if (isEndingTurn) return;
        isEndingTurn = true;
        StartCoroutine(EndTurnSequence());
    }

    public void StartTurn()
    {
        currentTurn++;
        // Update turn counter text
        if (turnCounterText != null)
        {
            turnCounterText.text = currentTurn.ToString();
        }
        gridManager.currentTurn = currentTurn;
        gridManager.SpawnNewRow();
        // Spawn pickups in the top row
        gridManager.SpawnPickups();
        gridManager.MoveBlocksDown();
        // Update block colors based on HP distribution
        gridManager.UpdateBlockColors();

        // Check if any block reached the bottom row
        if (gridManager.CheckGameOver())
        {
            GameOver();
            return;
        }

        // Update ball counter
        player.UpdateBallCounter();

        player.EnableShooting();
    }

    public void GameOver()
    {
        gameOverScreen.ShowGameOver(currentTurn);
    }

    private IEnumerator EndTurnSequence()
    {
        // Wait until all balls have landed
        yield return new WaitUntil(() => FindObjectsOfType<Ball>().Length == 0);
        yield return new WaitForSeconds(0.5f);

        // Notify all pickups that the turn has ended
        Pickup[] allPickups = FindObjectsOfType<Pickup>();
        foreach (Pickup pickup in allPickups)
        {
            pickup.OnTurnEnd();
        }

        isEndingTurn = false;
        StartTurn();
    }
}
