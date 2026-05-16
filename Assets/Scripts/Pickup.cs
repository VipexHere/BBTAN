using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Type of pickup
    public enum PickupType
    {
        Plus,
        Scatter,
        HorizontalStrike,
        VerticalStrike
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

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetVisuals();
    }

    void SetVisuals()
    {
        // Hide all symbols first
        symbolPlus.SetActive(false);
        symbolScatter.SetActive(false);
        symbolHStrike.SetActive(false);
        symbolVStrike.SetActive(false);

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
        }
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
                    other.GetComponent<Rigidbody2D>().velocity = randomDirection * other.GetComponent<Ball>().speed;
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
            usedThisTurn = false;
        }
    }
}
