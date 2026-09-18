using UnityEngine;

public class FadeOut : MonoBehaviour
{
    private float duration;
    private float elapsed = 0f;
    private SpriteRenderer[] renderers;
    private Color[] startColors;

    public void Init(float fadeDuration)
    {
        duration = fadeDuration;
        // Get all SpriteRenderers in children
        renderers = GetComponentsInChildren<SpriteRenderer>();
        startColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            startColors[i] = renderers[i].color;
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = new Color(startColors[i].r, startColors[i].g, startColors[i].b, alpha);
        }

        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}