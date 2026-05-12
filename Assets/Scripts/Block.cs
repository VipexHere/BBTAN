using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    // Aktualna liczba HP bloku (ile razy jeszcze musi zostać trafiony)
    public int health;

    // Referencja do komponentu tekstowego który wyświetla liczbę HP
    private TextMeshPro healthText;

    // Referencja do komponentu SpriteRenderer który kontroluje wygląd bloku
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Pobieramy komponenty których będziemy używać
        // GetComponentInChildren szuka komponentu również w obiektach potomnych
        healthText = GetComponentInChildren<TextMeshPro>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Ta metoda ustawia HP bloku i od razu aktualizuje wyświetlany tekst
    public void SetHealth(int value)
    {
        health = value;
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
            // Aktualizujemy wygląd bloku
            UpdateVisuals();
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

        // Na razie ustawiamy stały kolor – system kolorów zrobimy później
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
    }
}