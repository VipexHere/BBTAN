using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    // Pozycja startowa gracza (ustalana na początku gry)
    private Vector2 startPosition;

    // Czy gracz może teraz strzelać?
    private bool canShoot = true;

    // Referencja do prefaba piłki
    public GameObject ballPrefab;

    // Referencja do skryptu GridManager
    private GridManager gridManager;

    private TurnManager turnManager;

    // Ile piłek ma gracz
    public int ballCount = 1;

    // Ile piłek jeszcze nie wylądowało
    private int ballsInFlight = 0;

    // Liczba kropek podglądu trajektorii
    private int dotCount = 22;

    // Tablica przechowująca obiekty kropek
    private GameObject[] dots;

    // Reference to the LineRenderer component
    private LineRenderer lineRenderer;

    // Reference to the arrow head object
    private GameObject arrowHead;

    // Sprite for the arrow head (assign triangle sprite in Inspector)
    public Sprite arrowHeadSprite;

    // Position where mouse button was clicked
    private Vector2 clickPosition;

    // Position where the first ball landed
    private Vector2 firstBallLandingPosition;

    // Has the first ball already landed this turn?
    private bool firstBallLanded = false;

    // Marker showing where player will move after turn
    private GameObject landingMarker;

    void Start()
    {
        startPosition = transform.position;
        gridManager = FindObjectOfType<GridManager>();
        turnManager = FindObjectOfType<TurnManager>();

        // Create dots array
        dots = new GameObject[dotCount];

        // Create each dot in a loop
        for (int i = 0; i < dotCount; i++)
        {
            // Create dot as 2D circle sprite
            GameObject dot = new GameObject("Dot" + i);
            SpriteRenderer dotRenderer = dot.AddComponent<SpriteRenderer>();
            dotRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
            dotRenderer.color = Color.white;

            // Set dot size
            dot.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            // Hide dot on start
            dot.SetActive(false);

            // Save dot to array
            dots[i] = dot;
        }

        // Get LineRenderer component - POZA pętlą, tworzy się tylko raz
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        // Create arrow head - POZA pętlą, tworzy się tylko raz
        arrowHead = new GameObject("ArrowHead");
        SpriteRenderer arrowRenderer = arrowHead.AddComponent<SpriteRenderer>();
        arrowRenderer.sprite = arrowHeadSprite;
        arrowRenderer.color = Color.white;
        arrowHead.transform.localScale = new Vector3(0.15f, 0.4f, 1f);
        arrowHead.SetActive(false);

        // Create landing marker as a small circle
        landingMarker = new GameObject("LandingMarker");
        SpriteRenderer markerRenderer = landingMarker.AddComponent<SpriteRenderer>();
        markerRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
        markerRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        landingMarker.transform.localScale = new Vector3(0.29f, 0.29f, 1f);
        landingMarker.SetActive(false);
    }

    void Update()
    {
        if (canShoot)
        {
            HandleAiming();
        }
    }

    void HandleAiming()
    {
        // Get mouse position in world space
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate and normalize direction from player to mouse
        Vector2 direction = mousePosition - (Vector2)transform.position;
        direction.Normalize();

        // Prevent shooting downwards
        if (direction.y < 0)
        {
            HideDots();
            return;
        }

        // Save click position on mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            clickPosition = mousePosition;
        }

        // Show dots while mouse button is held
        if (Input.GetMouseButton(0))
        {
            ShowDots(mousePosition, direction);
        }

        // Shoot on mouse button release
        if (Input.GetMouseButtonUp(0))
        {
            HideDots();
            Shoot(direction);
        }
    }

    void ShowDots(Vector2 mousePosition, Vector2 direction)
    {
        // Calculate how far mouse moved down from click position
        float dragDistance = clickPosition.y - mousePosition.y;

        // Clamp drag distance to avoid extreme values
        dragDistance = Mathf.Clamp(dragDistance + 1.3f, 0.5f, 3.3f);

        // Show line from player towards aim direction
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, (Vector2)transform.position + direction * 1.8f);

        // Position arrow head at the end of the line
        arrowHead.SetActive(true);
        Vector2 arrowHeadPosition = (Vector2)transform.position + direction * 1.8f;
        arrowHead.transform.position = arrowHeadPosition;

        // Rotate arrow head to point in aim direction
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        arrowHead.transform.rotation = Quaternion.Euler(0, 0, -angle);

        // Calculate dot spacing and size based on mouse distance
        float dotSpacing = dragDistance * 0.15f;
        float dotSize = Mathf.Clamp(dragDistance * 0.03f, 0.03f, 0.08f);

        // Show and position each dot
        for (int i = 0; i < dotCount; i++)
        {
            // Calculate dot position along the direction
            float distance = 0.2f + (i + 1) * dotSpacing;
            Vector2 dotPosition = arrowHeadPosition + direction * distance;

            // Activate, position and scale the dot
            dots[i].SetActive(true);
            dots[i].transform.position = dotPosition;
            dots[i].transform.localScale = new Vector3(dotSize, dotSize, dotSize);
        }
    }

    void HideDots()
    {
        // Hide line
        lineRenderer.enabled = false;
        // Hide arrow head
        arrowHead.SetActive(false);

        // Deactivate all dots
        for (int i = 0; i < dotCount; i++)
        {
            dots[i].SetActive(false);
        }
    }

    public void Shoot(Vector2 direction)
    {
        canShoot = false;
        ballsInFlight = ballCount;
        StartCoroutine(ShootBalls(direction));
    }

    private IEnumerator ShootBalls(Vector2 direction)
    {
        for (int i = 0; i < ballCount; i++)
        {
            GameObject newBall = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            newBall.GetComponent<Ball>().Launch(direction);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetPosition(Vector2 newPosition)
    {
        Vector2 newPos = new Vector2(newPosition.x, transform.position.y);
        transform.position = newPos;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
    }

    public void EnableShooting()
    {
        canShoot = true;
    }

    public void OnBallLanded(Vector2 landingPosition)
    {
        // Save position of the first ball that landed
        if (!firstBallLanded)
        {
            firstBallLanded = true;
            firstBallLandingPosition = landingPosition;
            // Show landing marker at first ball landing position
            landingMarker.SetActive(true);
            landingMarker.transform.position = new Vector2(landingPosition.x, transform.position.y);
        }

        ballsInFlight--;

        // If all balls landed, move player and end turn
        if (ballsInFlight <= 0)
        {
            SetPosition(firstBallLandingPosition);
            firstBallLanded = false;
            // Hide landing marker when player moves
            landingMarker.SetActive(false);
            turnManager.OnShootingFinished();
        }
    }
}
