using UnityEngine;

public class Player : MonoBehaviour
{
    // Pozycja startowa gracza (ustalana na początku gry)
    private Vector2 startPosition;

    // Czy gracz może teraz strzelać?
    private bool canShoot = true;

    void Start()
    {
        startPosition = transform.position;
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
    }
}
