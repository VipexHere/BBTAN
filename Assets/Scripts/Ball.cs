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

    void Update()
    {
        // Check if ball is stuck moving horizontally
        if (rb.velocity.magnitude > 0)
        {
            // If vertical speed is very low, ball is moving almost horizontally
            if (Mathf.Abs(rb.velocity.y) < 0.1f)
            {
                // Add small upward nudge to prevent infinite horizontal bouncing
                rb.velocity = new Vector2(rb.velocity.x, 0.5f);
            }
        }
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
