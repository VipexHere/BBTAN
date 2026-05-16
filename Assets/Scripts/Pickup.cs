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

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it was a ball that hit the pickup
        if (other.CompareTag("Ball"))
        {
            // Handle pickup based on its type
            switch (pickupType)
            {
                case PickupType.Plus:
                    FindObjectOfType<Player>().ballCount++;
                    break;
            }

            // Destroy pickup after it's collected
            Destroy(gameObject);
        }
    }
}
