using UnityEngine;

public class CameraMovementForTest : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("No object with tag 'Player' found.");
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y, offset.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition + offset, smoothSpeed);
            transform.position = new Vector3(smoothedPosition.x,smoothedPosition.y,-10);
        }
    }
}