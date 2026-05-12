using UnityEngine;

public class Player : MonoBehaviour
{
    // Pozycja startowa gracza (ustalana na początku gry)
    private Vector2 startPosition;

    // Czy gracz może teraz strzelać?
    private bool canShoot = true;

    // Referencja do prefaba piłki
    public GameObject ballPrefab;

    // Referencja do skryptu GridManager
    private GridManager gridManager;

    void Start()
    {
        startPosition = transform.position;
        gridManager = FindObjectOfType<GridManager>();
    }

    void Update()
    {
        if (canShoot)
        {
            HandleAiming();
        }
    }

    void HandleAiming()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        direction.Normalize();
        if (direction.y < 0)
        {
            return;
        }
        Debug.DrawRay(transform.position, direction * 3f, Color.white);
        if (Input.GetMouseButtonDown(0))
        {
            Shoot(direction);
        }
    }

    public void Shoot(Vector2 direction)
    {
        canShoot = false;
        GameObject newBall = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        newBall.GetComponent<Ball>().Launch(direction);
    }

    public void SetPosition(Vector2 newPosition)
    {
        Vector2 newPos = new Vector2(newPosition.x, transform.position.y);
        transform.position = newPos;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
    }

    public void EnableShooting()
    {
        canShoot = true;
    }
}
