using UnityEngine;

public class TextBouncer : MonoBehaviour
{
    public float hoverSpeed = 2f; // Speed of the hover
    public float hoverHeight = 0.5f; // Height of the hover

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position; // Store the initial position
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
