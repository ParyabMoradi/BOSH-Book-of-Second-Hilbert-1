using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    public SpriteRenderer sr;
    public float speed = 2f;
    private float baseAlpha = 0.5f;

    void Update()
    {
        float alpha = baseAlpha + Mathf.Sin(Time.time * speed) * 0.3f;
        var color = sr.color;
        color.a = Mathf.Clamp01(alpha);
        sr.color = color;
    }
}
