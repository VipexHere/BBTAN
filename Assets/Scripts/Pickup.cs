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
