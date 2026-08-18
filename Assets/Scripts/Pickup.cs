using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Type of pickup
    public enum PickupType
    {
        Plus,
        Scatter,
        HorizontalStrike,
        VerticalStrike,
        Bomb,
        Sniper
    }

    // Which type is this pickup
    public PickupType pickupType;

    // Has this pickup been used this turn?
    private bool usedThisTurn = false;

    // Reference to the SpriteRenderer component
    private SpriteRenderer spriteRenderer;

    // References to symbol objects
    public GameObject symbolPlus;
    public GameObject symbolScatter;
    public GameObject symbolHStrike;
    public GameObject symbolVStrike;
    public GameObject symbolBomb;

    // Duration of the laser effect in seconds
    public float laserDuration = 0.3f;

    public Sprite squareSprite;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(PickupType type)
    {
        pickupType = type;
        SetVisuals();
    }

    void SetVisuals()
    {
        // Hide all symbols first
        symbolPlus.SetActive(false);
        symbolScatter.SetActive(false);
        symbolHStrike.SetActive(false);
        symbolVStrike.SetActive(false);
        symbolBomb.SetActive(false);

        switch (pickupType)
        {
            case PickupType.Plus:
                spriteRenderer.color = new Color(1f, 1f, 0f);
                symbolPlus.SetActive(true);
                break;
            case PickupType.Scatter:
                spriteRenderer.color = new Color(1f, 0f, 1f);
                symbolScatter.SetActive(true);
                break;
            case PickupType.HorizontalStrike:
                spriteRenderer.color = new Color(0f, 0.87f, 1f);
                symbolHStrike.SetActive(true);
                break;
            case PickupType.VerticalStrike:
                spriteRenderer.color = new Color(1f, 0.55f, 1f);
                symbolVStrike.SetActive(true);
                break;
            case PickupType.Bomb:
                spriteRenderer.color = new Color(1f, 0.3f, 0f);
                symbolBomb.SetActive(true);
                break;
        }
    }

    void ShowLaser(bool isHorizontal)
    {
        // Create a new GameObject for the laser
        GameObject laser = new GameObject("Laser");
        LineRenderer lr = laser.AddComponent<LineRenderer>();

        // Set laser appearance
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = Color.yellow;
        lr.sortingLayerName = "Default";
        lr.sortingOrder = 10;

        // Set laser position based on type
        if (isHorizontal)
        {
            lr.SetPosition(0, new Vector3(-3.5f, transform.position.y, -1));
            lr.SetPosition(1, new Vector3(3.5f, transform.position.y, -1));
        }
        else
        {
            lr.SetPosition(0, new Vector3(transform.position.x, -4.5f, -1));
            lr.SetPosition(1, new Vector3(transform.position.x, 4.5f, -1));
        }

        // Destroy laser after duration
        Destroy(laser, laserDuration);
    }

    void ShowExplosion()
    {
        // Create a new GameObject for the explosion circle
        GameObject explosion = new GameObject("Explosion");
        SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();

        // Set explosion appearance
        sr.sprite = squareSprite;
        sr.color = new Color(1f, 0.3f, 0f, 0.5f);
        sr.sortingOrder = 10;

        // Set explosion size to match bomb radius
        explosion.transform.position = transform.position;
        explosion.transform.localScale = new Vector3(2f, 2f, 1f);

        // Destroy explosion after duration
        Destroy(explosion, laserDuration);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            switch (pickupType)
            {
                case PickupType.Plus:
                    // Add one ball to player's count
                    FindObjectOfType<Player>().ballCount++;
                    Destroy(gameObject);
                    break;

                case PickupType.Scatter:
                    // Launch ball in random upward direction
                    float randomAngle = Random.Range(-90f, 90f);
                    Vector2 randomDirection = new Vector2(
                        Mathf.Sin(randomAngle * Mathf.Deg2Rad),
                        Mathf.Cos(randomAngle * Mathf.Deg2Rad)
                    );
                    other.GetComponent<Rigidbody2D>().linearVelocity = randomDirection * other.GetComponent<Ball>().speed;
                    usedThisTurn = true;
                    break;

                case PickupType.HorizontalStrike:
                    // Deal 1 damage to all blocks in the same row
                    float strikeY = transform.position.y;
                    Block[] allBlocks = FindObjectsOfType<Block>();
                    foreach (Block block in allBlocks)
                    {
                        if (Mathf.Abs(block.transform.position.y - strikeY) < 0.1f)
                        {
                            block.TakeDamage(1);
                        }
                    }
                    ShowLaser(true);
                    usedThisTurn = true;
                    break;

                case PickupType.VerticalStrike:
                    // Deal 1 damage to all blocks in the same column
                    float strikeX = transform.position.x;
                    Block[] allBlocksV = FindObjectsOfType<Block>();
                    foreach (Block block in allBlocksV)
                    {
                        if (Mathf.Abs(block.transform.position.x - strikeX) < 0.1f)
                        {
                            block.TakeDamage(1);
                        }
                    }
                    ShowLaser(false);
                    usedThisTurn = true;
                    break;

                case PickupType.Bomb:
                    // Deal 1 damage to all blocks within radius
                    float bombRadius = 1.5f;
                    Block[] allBlocksBomb = FindObjectsOfType<Block>();
                    foreach (Block block in allBlocksBomb)
                    {
                        float distance = Vector2.Distance(transform.position, block.transform.position);
                        if (distance <= bombRadius)
                        {
                            block.TakeDamage(1);
                        }
                    }
                    ShowExplosion();
                    usedThisTurn = true;
                    break;
            }
        }
    }

    // Called at the end of each turn to reset or destroy pickup
    public void OnTurnEnd()
    {
        if (usedThisTurn)
        {
            Destroy(gameObject);
        }
        else
        {
            // Destroy pickup if it has fallen below the floor
            if (transform.position.y <= -3.55f)
            {
                Destroy(gameObject);
            }
            else
            {
                usedThisTurn = false;
            }
        }
    }
}
