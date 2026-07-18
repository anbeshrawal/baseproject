using UnityEngine;

/// <summary>
/// A helper script attached to temporary ghost/after-image objects to fade them out and destroy them.
/// This creates the motion blur/trailing effect.
/// </summary>
public class FeatherGhost : MonoBehaviour
{
    private SpriteRenderer sr;
    private float lifetime;
    private float maxLifetime;
    private float startAlpha;
    private Color baseColor;

    public void Init(float fadeDuration, float initialAlpha, Color color)
    {
        sr = GetComponent<SpriteRenderer>();
        maxLifetime = fadeDuration;
        lifetime = fadeDuration;
        startAlpha = initialAlpha;
        baseColor = color;

        if (sr != null)
        {
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, startAlpha);
        }
    }

    void Update()
    {
        if (sr == null)
        {
            Destroy(gameObject);
            return;
        }

        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
        else
        {
            // Smoothly fade alpha to 0 over lifetime
            float progress = lifetime / maxLifetime;
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, progress * startAlpha);
        }
    }
}
