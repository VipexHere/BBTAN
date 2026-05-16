using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    // Referencje do innych skryptów
    private GridManager gridManager;
    private Player player;

    // Numer aktualnej tury
    private int currentTurn = 0;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        player = FindObjectOfType<Player>();
        StartTurn();
    }

    public void OnShootingFinished()
    {
        StartCoroutine(EndTurnSequence());
    }

    public void StartTurn()
    {
        currentTurn++;
        gridManager.currentTurn = currentTurn;
        gridManager.SpawnNewRow();
        gridManager.MoveBlocksDown();
        player.EnableShooting();
    }

    private IEnumerator EndTurnSequence()
    {
        yield return new WaitForSeconds(0.5f);
        StartTurn();
    }
}
