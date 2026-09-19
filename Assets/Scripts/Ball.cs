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

    // Icon reference for sniper hit effect
    public GameObject sniperIcon;

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
                if (sniperIcon != null)
                {
                    GameObject burst = Instantiate(sniperIcon, collision.transform.position, Quaternion.identity);
                    burst.SetActive(true);
                    burst.transform.localScale = new Vector3(2.2f, 2.2f, 1f);
                    foreach (SpriteRenderer sr in burst.GetComponentsInChildren<SpriteRenderer>())
                    {
                        Color c = sr.color;
                        sr.color = new Color(c.r, c.g, c.b, 0.7f);
                    }
                    FadeOut fadeOut = burst.AddComponent<FadeOut>();
                    fadeOut.Init(0.5f);
                }

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
