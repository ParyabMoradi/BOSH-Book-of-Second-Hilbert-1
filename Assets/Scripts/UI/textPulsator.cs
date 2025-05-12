using UnityEngine;

public class textPulsator : MonoBehaviour
{
    public float pulsateSpeed = 1f; // Speed of the pulsation
    public float pulsateAmount = 0.1f; // Amount of size change

    private Vector3 originalScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale; // Store the original scale of the text
    }

    // Update is called once per frame
    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * pulsateSpeed) * pulsateAmount;
        transform.localScale = originalScale * scale;
    }
}
