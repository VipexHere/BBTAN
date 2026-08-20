using UnityEngine;

public class Ball : MonoBehaviour
{
    // Prędkość piłki
    public float speed = 10f;

    // Referencja do komponentu Rigidbody2D
    private Rigidbody2D rb;

    // Does this ball deal 10x damage?
    public bool isSniperBall = false;

    // Was this ball created by a multiplier?
    public bool isMultiplierBall = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        // Check if ball is stuck moving horizontally
        if (rb.linearVelocity.magnitude > 0)
        {
            // If vertical speed is very low, ball is moving almost horizontally
            if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                // Add small upward nudge to prevent infinite horizontal bouncing
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0.5f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            int damage = isSniperBall ? 10 : 1;
            collision.gameObject.GetComponent<Block>().TakeDamage(damage);
            
            // Reset sniper after first hit
            if (isSniperBall)
            {
                // Show sniper hit effect
                GameObject hitEffect = new GameObject("HitEffect");
                SpriteRenderer hitSr = hitEffect.AddComponent<SpriteRenderer>();
                hitSr.sprite = GetComponent<SpriteRenderer>().sprite;
                hitSr.color = new Color(1f, 0f, 0f, 0.7f);
                hitSr.sortingOrder = 10;
                hitEffect.transform.position = collision.transform.position;
                hitEffect.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                Destroy(hitEffect, 0.1f);

                isSniperBall = false;
                GetComponent<SpriteRenderer>().color = Color.white;
            }
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
