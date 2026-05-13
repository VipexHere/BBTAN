using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    // Referencje do innych skryptów
    private GridManager gridManager;
    private Player player;

    // Numer aktualnej tury
    private int currentTurn = 1;

    // Czy tura jest aktualnie w toku?
    private bool isTurnInProgress = false;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        player = FindObjectOfType<Player>();
    }

    public void OnShootingFinished()
    {
        StartCoroutine(EndTurnSequence());
    }

    public void StartTurn()
    {
        isTurnInProgress = true;
        currentTurn++;
        gridManager.currentTurn = currentTurn;
        gridManager.SpawnNewRow();
        gridManager.MoveBlocksDown();
        player.EnableShooting();
    }

    private IEnumerator EndTurnSequence()
    {
        isTurnInProgress = false;
        yield return new WaitForSeconds(0.5f);
        StartTurn();
    }
}
