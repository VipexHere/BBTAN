using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    // Referencje do innych skryptów
    private GridManager gridManager;
    private Player player;

    // Numer aktualnej tury
    private int currentTurn = 0;

    // Prevents EndTurnSequence from running multiple times
    private bool isEndingTurn = false;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        player = FindObjectOfType<Player>();
        StartTurn();
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
        gridManager.currentTurn = currentTurn;
        gridManager.SpawnNewRow();
        // Spawn pickups in the top row
        gridManager.SpawnPickups();
        gridManager.MoveBlocksDown();

        // Check if any block reached the bottom row
        if (gridManager.CheckGameOver())
        {
            GameOver();
            return;
        }

        player.EnableShooting();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }

    private IEnumerator EndTurnSequence()
    {
        // Wait until there are no balls left in the scene
        yield return new WaitUntil(() => FindObjectsOfType<Ball>().Length == 0);
        yield return new WaitForSeconds(0.5f);
        isEndingTurn = false;
        StartTurn();
    }
}
