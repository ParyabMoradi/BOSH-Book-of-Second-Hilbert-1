using UnityEngine;
using UnityEngine.UI;

public class BackgroundScaling : MonoBehaviour
{
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        float xmas = Screen.width * Camera.main.orthographicSize * 2.0f / (Screen.height * 1.0f);
        float yScale = Camera.main.orthographicSize * 2.0f / sr.bounds.size.y;
        float xScale = 0;
        if (Screen.height > Screen.width)
            xScale = xmas / sr.bounds.size.x;
        else
            xScale = 1.5f; // for web view etc. you can change 1.5 according to your needs
        transform.localScale = new Vector3(xScale, yScale, 1); // Using 2D so z isn't needed.
    }
}