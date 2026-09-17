using UnityEngine;

public class FadeOut : MonoBehaviour
{
    private float duration;
    private float elapsed = 0f;
    private SpriteRenderer sr;
    private Color startColor;

    public void Init(float fadeDuration)
    {
        duration = fadeDuration;
        sr = GetComponentInChildren<SpriteRenderer>();
        startColor = sr.color;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
        sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
