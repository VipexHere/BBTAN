using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
        Sniper,
        Multiplier,
        MegaBomb,
        Lightning,
        Fire,
        Freeze
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
    public GameObject symbolSniper;
    public GameObject symbolMultiplier;
    public GameObject symbolMegaBomb;
    public GameObject symbolLightning;
    public GameObject symbolFire;
    public GameObject symbolFreeze;
    // Icon-only reference for charged burst effect
    public GameObject symbolLightningIcon;
    // Icon-only reference for charged burst effect
    public GameObject symbolFreezeIcon;
    // Icon-only reference for charged burst effect
    public GameObject symbolMegaBombIcon;

    // Duration of the laser effect in seconds
    public float laserDuration = 0.3f;

    public Sprite squareSprite;

    // Counter for sniper uses remaining
    private int sniperUsesLeft = 0;

    // Reference to sniper counter text
    public TextMeshPro sniperCounterText;

    // Counter for mega bomb charges
    private int megaBombChargesLeft = 0;

    // Reference to mega bomb counter text
    public TextMeshPro megaBombCounterText;

    // References to elemental counter texts
    public TextMeshPro lightningCounterText;
    public TextMeshPro fireCounterText;
    public TextMeshPro freezeCounterText;

    // Number of chain jumps per Lightning activation
    private int lightningJumps = 3;

    private int lightningChargesNeeded = 0;
    private int lightningChargesCurrent = 0;

    private int fireChargesNeeded = 0;
    private int fireChargesCurrent = 0;

    private int freezeChargesNeeded = 0;
    private int freezeChargesCurrent = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(PickupType type)
    {
        pickupType = type;
        SetVisuals();

        // Initialize sniper counter
        if (pickupType == PickupType.Sniper)
        {
            int ballCount = FindObjectOfType<Player>().ballCount;
            sniperUsesLeft = Mathf.CeilToInt(ballCount / 2f);
            if (sniperCounterText != null)
            {
                sniperCounterText.text = sniperUsesLeft.ToString();
            }
        }
        else if (pickupType == PickupType.MegaBomb)
        {
            int ballCount = FindObjectOfType<Player>().ballCount;
            megaBombChargesLeft = Mathf.CeilToInt(ballCount * 1.5f);
            if (megaBombCounterText != null)
            {
                megaBombCounterText.text = megaBombChargesLeft.ToString();
            }
        }
        else if (pickupType == PickupType.Lightning)
        {
            int ballCount = FindObjectOfType<Player>().ballCount;
            lightningChargesNeeded = Mathf.CeilToInt(ballCount * 1.5f);
            if (lightningCounterText != null)
            {
                lightningCounterText.text = lightningChargesNeeded.ToString();
            }
        }
        else if (pickupType == PickupType.Fire)
        {
            int ballCount = FindObjectOfType<Player>().ballCount;
            fireChargesNeeded = Mathf.CeilToInt(ballCount * 1.5f);
            if (fireCounterText != null)
            {
                fireCounterText.text = fireChargesNeeded.ToString();
            }
        }
        else if (pickupType == PickupType.Freeze)
        {
            int ballCount = FindObjectOfType<Player>().ballCount;
            freezeChargesNeeded = Mathf.CeilToInt(ballCount * 1.5f);
            if (freezeCounterText != null)
            {
                freezeCounterText.text = freezeChargesNeeded.ToString();
            }
        }
        else
        {
            if (sniperCounterText != null)
            {
                sniperCounterText.gameObject.SetActive(false);
            }
        }

    }

    void SetVisuals()
    {
        // Hide all symbols first
        symbolPlus.SetActive(false);
        symbolScatter.SetActive(false);
        symbolHStrike.SetActive(false);
        symbolVStrike.SetActive(false);
        symbolBomb.SetActive(false);
        symbolSniper.SetActive(false);
        symbolMultiplier.SetActive(false);
        symbolMegaBomb.SetActive(false);
        symbolLightning.SetActive(false);
        symbolFire.SetActive(false);
        symbolFreeze.SetActive(false);

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
                spriteRenderer.color = new Color(0f, 1f, 0.55f);
                symbolVStrike.SetActive(true);
                break;
            case PickupType.Bomb:
                spriteRenderer.color = new Color(0.6f, 0f, 0f);
                symbolBomb.SetActive(true);
                break;
            case PickupType.Sniper:
                spriteRenderer.color = new Color(1f, 0.6f, 0f);
                symbolSniper.SetActive(true);
                break;
            case PickupType.Multiplier:
                spriteRenderer.color = new Color(1f, 0f, 0.49f);
                symbolMultiplier.SetActive(true);
                break;
            case PickupType.MegaBomb:
                spriteRenderer.color = new Color(0.41f, 0.41f, 0.41f);
                symbolMegaBomb.SetActive(true);
                break;
            case PickupType.Lightning:
                spriteRenderer.color = new Color(1f, 1f, 0.6f);
                symbolLightning.SetActive(true);
                break;
            case PickupType.Fire:
                spriteRenderer.color = new Color(0.96f, 0.17f, 0f);
                symbolFire.SetActive(true);
                break;
            case PickupType.Freeze:
                spriteRenderer.color = new Color(0.4f, 0.8f, 1f);
                symbolFreeze.SetActive(true);
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

    void ShowMegaExplosion()
    {
        // Create explosion covering the whole map
        GameObject explosion = new GameObject("MegaExplosion");
        SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();

        // Use square sprite to cover whole grid
        sr.sprite = squareSprite;
        sr.color = new Color(1f, 0.3f, 0f, 0.4f);
        sr.sortingOrder = 10;

        // Size to cover entire grid (7x9)
        explosion.transform.position = new Vector3(0, 0, 0);
        explosion.transform.localScale = new Vector3(7f, 9f, 1f);

        // Destroy after duration
        Destroy(explosion, 0.3f);
    }

    void ShowLightning(List<Block> targets, float width, Color color)
    {
        // Create a new GameObject for the lightning chain
        GameObject lightning = new GameObject("Lightning");
        LineRenderer lr = lightning.AddComponent<LineRenderer>();

        // Set lightning appearance — thin line, bright yellow-white color
        lr.startWidth = width;
        lr.endWidth = width;
        lr.positionCount = targets.Count + 1;
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = color;
        lr.sortingLayerName = "Default";
        lr.sortingOrder = 10;

        // First point is the pickup itself
        lr.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1));

        // Each subsequent point is a hit block
        for (int i = 0; i < targets.Count; i++)
        {
            lr.SetPosition(i + 1, new Vector3(targets[i].transform.position.x, targets[i].transform.position.y, -1));
        }

        // Destroy lightning visual after short duration
        Destroy(lightning, width > 0.1f ? laserDuration + 0.1f : laserDuration - 0.02f);
    }

    List<Block> GetLightningTargets(int jumps)
    {
        // Get all blocks currently on the board
        Block[] allBlocks = FindObjectsOfType<Block>();

        // If no blocks, return empty list
        if (allBlocks.Length == 0) return new List<Block>();

        // List of blocks already hit in this chain — no repeats allowed
        List<Block> hitBlocks = new List<Block>();

        // List of remaining available targets
        List<Block> available = new List<Block>(allBlocks);

        // Pick random blocks one by one up to lightningJumps
        for (int i = 0; i < jumps; i++)
        {
            // Stop if no more blocks available
            if (available.Count == 0) break;

            // Pick a random block from available
            int randomIndex = Random.Range(0, available.Count);
            Block chosen = available[randomIndex];

            // Add to hit list and remove from available
            hitBlocks.Add(chosen);
            available.Remove(chosen);
        }

        return hitBlocks;
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

                case PickupType.Sniper:

                    // Enhance ball and decrease counter
                    other.GetComponent<Ball>().isSniperBall = true;
                    other.GetComponent<SpriteRenderer>().color = Color.red;
                    sniperUsesLeft--;

                    // Update counter text
                    if (sniperCounterText != null)
                    {
                        sniperCounterText.text = sniperUsesLeft.ToString();
                    }

                    // Destroy when uses run out
                    if (sniperUsesLeft <= 0)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        usedThisTurn = true;
                    }
                    break;

                case PickupType.Multiplier:
                    // Prevent multiplier balls from triggering again
                    if (other.GetComponent<Ball>().isMultiplierBall) break;

                    // Get current ball velocity
                    Vector2 currentVelocity = other.GetComponent<Rigidbody2D>().linearVelocity;
                    float ballSpeed = other.GetComponent<Ball>().speed;

                    // Create two new balls at slight angles
                    float spreadAngle = 45f;

                    // Rotate velocity left
                    Vector2 leftDirection = Quaternion.Euler(0, 0, spreadAngle) * currentVelocity.normalized;
                    GameObject leftBall = Instantiate(FindObjectOfType<Player>().ballPrefab, transform.position, Quaternion.identity);
                    leftBall.GetComponent<Ball>().Launch(leftDirection);

                    // Rotate velocity right
                    Vector2 rightDirection = Quaternion.Euler(0, 0, -spreadAngle) * currentVelocity.normalized;
                    GameObject rightBall = Instantiate(FindObjectOfType<Player>().ballPrefab, transform.position, Quaternion.identity);
                    rightBall.GetComponent<Ball>().Launch(rightDirection);

                    leftBall.GetComponent<Ball>().isMultiplierBall = true;
                    rightBall.GetComponent<Ball>().isMultiplierBall = true;

                    // Increase balls in flight count
                    // +2 new balls, -1 original = net +1
                    FindObjectOfType<Player>().ballsInFlight += 1;

                    // Destroy original ball - replaced by two new ones
                    Destroy(other.gameObject);
                    usedThisTurn = true;
                    break;

                case PickupType.MegaBomb:
                    // Decrease charge counter
                    megaBombChargesLeft--;

                    // Update counter text
                    if (megaBombCounterText != null)
                    {
                        megaBombCounterText.text = megaBombChargesLeft.ToString();
                    }

                    // Explode when fully charged
                    if (megaBombChargesLeft <= 0)
                    {
                        // Find max HP of non-double blocks
                        int maxHP = 0;
                        Block[] allBlocksMega = FindObjectsOfType<Block>();
                        foreach (Block block in allBlocksMega)
                        {
                            if (!block.isDouble && block.health > maxHP)
                            {
                                maxHP = block.health;
                            }
                        }

                        // Deal damage to ALL blocks
                        foreach (Block block in allBlocksMega)
                        {
                            block.TakeDamage(maxHP);
                        }

                        // Show charged burst effect
                        if (symbolMegaBombIcon != null)
                        {
                            GameObject burst = Instantiate(symbolMegaBombIcon, transform.position, Quaternion.identity);
                            burst.transform.localScale = new Vector3(8f, 8f, 1f);
                            foreach (SpriteRenderer sr in burst.GetComponentsInChildren<SpriteRenderer>())
                            {
                                Color c = sr.color;
                                sr.color = new Color(c.r, c.g, c.b, 0.7f);
                                sr.material = new Material(Shader.Find("Sprites/Default"));
                                sr.material.SetFloat("_Mode", 1);
                            }
                            FadeOut fadeOut = burst.AddComponent<FadeOut>();
                            fadeOut.Init(0.5f);
                        }

                        // Show explosion effect covering whole map
                        ShowMegaExplosion();
                        Destroy(gameObject);
                    }
                    else
                    {
                        usedThisTurn = true;
                    }
                    break;

                case PickupType.Lightning:
                        lightningChargesCurrent++;
                        if (lightningChargesCurrent >= lightningChargesNeeded)
                        {
                            // Last charge — fire charged effect immediately
                            lightningChargesCurrent = 0;
                            lightningChargesNeeded = int.MaxValue;
                            if (lightningCounterText != null)
                            {
                                lightningCounterText.gameObject.SetActive(false);
                            }
                            // Show charged burst effect
                            if (symbolLightningIcon != null)
                            {
                                GameObject burst = Instantiate(symbolLightningIcon, transform.position, Quaternion.identity);
                                // Set alpha on all sprite renderers
                                foreach (SpriteRenderer sr in burst.GetComponentsInChildren<SpriteRenderer>())
                                {
                                    Color c = sr.color;
                                    sr.color = new Color(c.r, c.g, c.b, 0.7f);
                                }
                                burst.transform.localScale = new Vector3(7f, 7f, 1f);
                                FadeOut fadeOut = burst.AddComponent<FadeOut>();
                                fadeOut.Init(0.5f);
                            }
                        List<Block> chargedTargets = GetLightningTargets(lightningJumps * 2);
                            foreach (Block block in chargedTargets)
                            {
                                block.TakeDamage(2);
                            }
                            ShowLightning(chargedTargets, 0.14f, new Color(0.4f, 0.7f, 1f));
                        }
                        else
                        {
                            if (lightningCounterText != null)
                            {
                                lightningCounterText.text = (lightningChargesNeeded - lightningChargesCurrent).ToString();
                            }
                            List<Block> lightningTargets = GetLightningTargets(lightningJumps);
                            foreach (Block block in lightningTargets)
                            {
                                block.TakeDamage(1);
                            }
                            ShowLightning(lightningTargets, 0.08f, new Color(1f, 1f, 0.6f));
                        }
                    usedThisTurn = true;
                    break;

                case PickupType.Fire:
                    fireChargesCurrent++;
                    if (fireChargesCurrent >= fireChargesNeeded)
                    {
                        // Charged effect — deal fire damage to all burning blocks
                        fireChargesCurrent = 0;
                        fireChargesNeeded = int.MaxValue;
                        if (fireCounterText != null)
                        {
                            fireCounterText.gameObject.SetActive(false);
                        }
                        // Show charged burst effect
                        if (symbolFire != null)
                        {
                            GameObject burst = new GameObject("FireBurstEffect");
                            SpriteRenderer burstSr = burst.AddComponent<SpriteRenderer>();
                            burstSr.sprite = symbolFire.GetComponentInChildren<SpriteRenderer>().sprite;
                            burstSr.color = new Color(1f, 1f, 1f, 0.7f);
                            burstSr.sortingOrder = 10;
                            burst.transform.position = transform.position;
                            burst.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                            FadeOut fadeOut = burst.AddComponent<FadeOut>();
                            fadeOut.Init(0.5f);
                        }
                        Block[] allBlocksFire = FindObjectsOfType<Block>();
                        foreach (Block block in allBlocksFire)
                        {
                            if (block.fireStacks > 0)
                            {
                                block.OnFireDamage();
                            }
                        }
                    }
                    else
                    {
                        // Base effect — add fire stack to random block
                        if (fireCounterText != null)
                        {
                            fireCounterText.text = (fireChargesNeeded - fireChargesCurrent).ToString();
                        }
                        Block[] allBlocksF = FindObjectsOfType<Block>();
                        if (allBlocksF.Length > 0)
                        {
                            int randomIndex = Random.Range(0, allBlocksF.Length);
                            allBlocksF[randomIndex].AddFireStack();
                        }
                    }
                    usedThisTurn = true;
                    break;

                case PickupType.Freeze:
                    freezeChargesCurrent++;
                    if (freezeChargesCurrent >= freezeChargesNeeded)
                    {
                        freezeChargesCurrent = 0;
                        freezeChargesNeeded = int.MaxValue;
                        if (freezeCounterText != null)
                        {
                            freezeCounterText.gameObject.SetActive(false);
                        }
                        // Show charged burst effect
                        if (symbolFreezeIcon != null)
                        {
                            GameObject burst = Instantiate(symbolFreezeIcon, transform.position, Quaternion.identity);
                            burst.transform.localScale = new Vector3(6f, 6f, 1f);
                            foreach (SpriteRenderer sr in burst.GetComponentsInChildren<SpriteRenderer>())
                            {
                                Color c = sr.color;
                                sr.color = new Color(c.r, c.g, c.b, 0.7f);
                            }
                            FadeOut fadeOut = burst.AddComponent<FadeOut>();
                            fadeOut.Init(0.5f);
                        }
                        FindObjectOfType<TurnManager>().isFrozenThisTurn = true;
                        Block[] frozenBlocks = FindObjectsOfType<Block>();
                        foreach (Block block in frozenBlocks)
                        {
                            block.SetFreezeOverlay(true);
                        }
                    }
                    else
                    {
                        if (freezeCounterText != null)
                        {
                            freezeCounterText.text = (freezeChargesNeeded - freezeChargesCurrent).ToString();

                            // Base effect - add one ice stack to a random block
                            Block[] allBlocksF = FindObjectsOfType<Block>();
                            if (allBlocksF.Length > 0)
                            {
                                int randomIndex = Random.Range(0, allBlocksF.Length);
                                allBlocksF[randomIndex].AddIceStack();
                            }
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
