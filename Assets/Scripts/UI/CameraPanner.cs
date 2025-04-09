using UnityEngine;

public class CameraPanner : MonoBehaviour
{
    // Variables to control the camera panning
    public float panSpeed = 1f; // Speed of the camera panning
    public float panDistance = 5f; // Distance to pan the camera

    private Vector3 startPosition;

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the initial position of the camera
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new position using a sine wave
        float offset = Mathf.Sin(Time.time * panSpeed) * panDistance;
        transform.position = new Vector3(startPosition.x + offset, startPosition.y, startPosition.z);
    }
}
