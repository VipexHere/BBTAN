using UnityEngine;

public class Ball : MonoBehaviour
{
    // Prędkość piłki
    public float speed = 10f;

    // Referencja do komponentu Rigidbody2D
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
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
            // Inform player that a ball landed, pass landing position
            FindObjectOfType<Player>().OnBallLanded(transform.position);
            // Usuwamy piłkę ze sceny
            Destroy(gameObject);
        }
    }
}
