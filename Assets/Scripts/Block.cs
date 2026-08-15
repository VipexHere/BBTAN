using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    // Reference to GridManager
    private GridManager gridManager;

    // Aktualna liczba HP bloku (ile razy jeszcze musi zostać trafiony)
    public int health;

    // Referencja do komponentu tekstowego który wyświetla liczbę HP
    private TextMeshPro healthText;

    // Referencja do komponentu SpriteRenderer który kontroluje wygląd bloku
    private SpriteRenderer spriteRenderer;

    // Block shape type
    public enum BlockShape
    {
        Square,
        Triangle
    }

    // Current shape of this block
    public BlockShape shape = BlockShape.Square;

    // Rotation of the triangle (0, 90, 180, 270 degrees)
    public int triangleRotation = 0;

    // Triangle sprite
    public Sprite triangleSprite;

    // Is this a double block?
    public bool isDouble = false;

    void Awake()
    {
        // Pobieramy komponenty których będziemy używać
        // GetComponentInChildren szuka komponentu również w obiektach potomnych
        healthText = GetComponentInChildren<TextMeshPro>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get reference to GridManager
        gridManager = FindObjectOfType<GridManager>();
    }

    // Ta metoda ustawia HP bloku i od razu aktualizuje wyświetlany tekst
    public void SetHealth(int value)
    {
        health = value;
        UpdateVisuals();
    }

    public void SetShape(BlockShape newShape, int rotation = 0)
    {
        shape = newShape;
        triangleRotation = rotation;

        if (shape == BlockShape.Triangle)
        {
            // Set triangle rotation
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            // Counter rotate the text to keep it upright
            if (healthText != null)
            {
                healthText.transform.localRotation = Quaternion.Euler(0, 0, -rotation);
            }
            // Move text to center of triangle
            if (healthText != null)
            {
                healthText.transform.localPosition = new Vector3(-0.2f, -0.2f, 0);
            }
            // Change sprite to triangle
            spriteRenderer.sprite = triangleSprite;

            // Replace Box Collider with Polygon Collider
            Destroy(GetComponent<BoxCollider2D>());
            PolygonCollider2D polyCollider = gameObject.AddComponent<PolygonCollider2D>();

            // Define triangle points (right angle in bottom-left corner)
            Vector2[] points = new Vector2[]
            {
                new Vector2(-0.5f, -0.5f),
                new Vector2(0.5f, -0.5f),
                new Vector2(-0.5f, 0.5f)
            };
            polyCollider.SetPath(0, points);
            polyCollider.sharedMaterial = GetComponent<Collider2D>() != null ?
                GetComponent<Collider2D>().sharedMaterial : null;
        }
    }

    public void SetDouble(bool value)
    {
        isDouble = value;
        UpdateVisuals();
    }

    // Ta metoda jest wywoływana gdy piłka trafi w blok
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // Blok zniszczony – usuwamy go ze sceny
            Destroy(gameObject);
        }
        else
        {
            // Update visuals
            UpdateVisuals();
            // Update only this block's color based on current HP distribution
            UpdateOwnColor();
        }
    }

    // Aktualizuje tekst oraz kolor bloku
    private void UpdateVisuals()
    {
        // Ustawiamy tekst na aktualną wartość HP
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }

        // Set color based on block type
        if (spriteRenderer != null)
        {
            if (isDouble)
            {
                // Double blocks are blue
                spriteRenderer.color = Color.blue;
            }
            else
            {
                // Normal blocks are red
                spriteRenderer.color = Color.red;
            }
        }
    }

    public void UpdateColor(int minHP, int maxHP)
    {
        if (spriteRenderer == null) return;

        // Calculate where this block falls between min and max HP
        // t = 0 means lowest HP (yellow/cyan), t = 1 means highest HP (red/blue)
        // Minimum HP difference before color gradient kicks in
        int minDifference = 3;
        float t = (maxHP - minHP < minDifference) ? 1f : (float)(health - minHP) / (maxHP - minHP);

        if (isDouble)
        {
            // Double blocks: interpolate from cyan (low HP) to blue (high HP)
            spriteRenderer.color = Color.Lerp(Color.cyan, Color.blue, t);
        }
        else
        {
            // Normal blocks: interpolate from yellow (low HP) to red (high HP)
            spriteRenderer.color = Color.Lerp(new Color(1f, 0.8f, 0f), Color.red, t);
        }
    }

    void UpdateOwnColor()
    {
        // Find all blocks of the same type
        Block[] allBlocks = FindObjectsOfType<Block>();
        int minHP = int.MaxValue;
        int maxHP = int.MinValue;

        foreach (Block block in allBlocks)
        {
            // Only compare with blocks of the same type
            if (block.isDouble == isDouble)
            {
                if (block.health < minHP) minHP = block.health;
                if (block.health > maxHP) maxHP = block.health;
            }
        }

        // Update only this block's color
        UpdateColor(minHP, maxHP);
    }
}