using UnityEngine;

public class Ball : MonoBehaviour
{
    // Prędkość piłki
    public float speed = 10f;

    // Czy piłka jest aktualnie w ruchu?
    private bool isMoving = false;

    // Referencja do komponentu Rigidbody2D
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        isMoving = true;
        rb.velocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            collision.gameObject.GetComponent<Block>().TakeDamage(1);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Floor"))
        {
            isMoving = false;
            FindObjectOfType<Player>().EnableShooting();
            Destroy(gameObject);
        }
    }
}
