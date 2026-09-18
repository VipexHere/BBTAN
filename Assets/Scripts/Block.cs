using UnityEngine;
using TMPro;
using System.Collections;

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
        Triangle,
        Circle
    }

    // Current shape of this block
    public BlockShape shape = BlockShape.Square;

    // Rotation of the triangle (0, 90, 180, 270 degrees)
    public int triangleRotation = 0;

    // Triangle sprite
    public Sprite triangleSprite;

    // Circle sprite
    public Sprite circleSprite;

    // Is this a double block?
    public bool isDouble = false;

    // Number of fire stacks on this block
    public int fireStacks = 0;

    // Reference to fire visual group
    public GameObject fireGroup;

    // Reference to fire stack counter text
    public TextMeshPro fireCounterText;

    // Sprite used for fire hit effect
    public Sprite fireSprite;

    // Reference to freeze overlay
    public GameObject freezeOverlay;

    // Number of ice stacks on this block
    public int iceStacks = 0;

    // Reference to chill visual group
    public GameObject chillGroup;

    // Reference to chill stack counter text
    public TextMeshPro chillCounterText;

    // Icon-only reference for chill hit effect
    public GameObject chillIcon;

    void Awake()
    {
        // Pobieramy komponenty których będziemy używać
        // GetComponentInChildren szuka komponentu również w obiektach potomnych
        healthText = GetComponentInChildren<TextMeshPro>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get reference to GridManager
        gridManager = FindObjectOfType<GridManager>();

        if (fireGroup != null)
        {
            fireGroup.SetActive(false);
        }

        if (chillGroup != null)
        {
            chillGroup.SetActive(false);
        }
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
        else if (shape == BlockShape.Circle)
        {
            // Change sprite to circle
            spriteRenderer.sprite = circleSprite;

            // Replace Box Collider with Circle Collider
            Destroy(GetComponent<BoxCollider2D>());
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.5f;
            circleCollider.sharedMaterial = Resources.Load<PhysicsMaterial2D>("BallMaterial");
        }
    }

    public void SetDouble(bool value)
    {
        isDouble = value;
        UpdateVisuals();
    }

    // Ta metoda jest wywoływana gdy piłka trafi w blok
    public void TakeDamage(int damage, bool fromFire = false)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            // If hit by a ball and block is on fire, apply fire damage
            if (!fromFire && fireStacks > 0)
            {
                OnFireDamage();
            }
            UpdateVisuals();
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

    public void AddFireStack()
    {
        fireStacks++;
        UpdateFireVisual();
    }

    public void OnFireDamage()
    {
        if (fireStacks <= 0) return;

        // Show fire hit effect
        GameObject hitEffect = new GameObject("FireHitEffect");
        SpriteRenderer hitSr = hitEffect.AddComponent<SpriteRenderer>();
        hitSr.sprite = fireSprite;
        hitSr.color = new Color(1f, 1f, 1f, 0.7f);
        hitSr.sortingOrder = 10;
        hitEffect.transform.position = transform.position;
        hitEffect.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
        FadeOut fadeOut = hitEffect.AddComponent<FadeOut>();
        fadeOut.Init(0.5f);

        // Deal damage equal to fire stacks
        TakeDamage(fireStacks, true);

        // Reduce stacks by 1
        fireStacks--;
        UpdateFireVisual();
    }

    private void UpdateFireVisual()
    {
        if (fireGroup == null) return;

        if (fireStacks <= 0)
        {
            fireGroup.SetActive(false);
        }
        else
        {
            fireGroup.SetActive(true);
            fireGroup.transform.rotation = Quaternion.identity;
            if (fireCounterText != null)
            {
                fireCounterText.text = fireStacks.ToString();
            }
        }
    }

    // Adds one ice stack to this block and updates the overlay
    public void AddIceStack()
    {
        iceStacks++;
        UpdateChillVisual();
    }

    // Deals damage equal to ice stacks and clears them
    public void OnIceDamage()
    {
        if (iceStacks <= 0) return;

        Vector3 currentPosition = transform.position;
        int stackCount = iceStacks;

        // Clear ice stacks and hide visual
        iceStacks = 0;
        UpdateChillVisual();

        // Deal damage equal to number of ice stacks
        TakeDamage(stackCount);

        // If block survived show effect after delay, if died show immediately
        if (health > 0)
        {
            StartCoroutine(ShowChillHitEffectDelayed(0.05f));
        }
        else
        {
            ShowChillHitEffect(currentPosition);
        }
    }

    // Shows chill hit effect at given position
    private void ShowChillHitEffect(Vector3 position)
    {
        if (chillIcon != null)
        {
            GameObject burst = Instantiate(chillIcon, position, Quaternion.identity);
            burst.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            foreach (SpriteRenderer sr in burst.GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, 0.7f);
            }
            FadeOut fadeOut = burst.AddComponent<FadeOut>();
            fadeOut.Init(0.5f);
        }
    }

    // Shows chill hit effect after a delay
    private IEnumerator ShowChillHitEffectDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowChillHitEffect(transform.position);
    }

    // Updates the chill visual based on current ice stacks
    private void UpdateChillVisual()
    {
        if (chillGroup == null) return;

        if (iceStacks <= 0)
        {
            chillGroup.SetActive(false);
        }
        else
        {
            chillGroup.SetActive(true);
            chillGroup.transform.rotation = Quaternion.identity;
            if (chillCounterText != null)
            {
                chillCounterText.text = iceStacks.ToString();
            }
        }
    }

    public void SetFreezeOverlay(bool visible)
    {
        if (freezeOverlay != null)
        {
            freezeOverlay.SetActive(visible);
        }
    }
}