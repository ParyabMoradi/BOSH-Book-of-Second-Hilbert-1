using UnityEngine;
using Unity.Netcode;

public class TheBookDetectorArea : NetworkBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject wall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return; // Only the server performs this logic

        if (collision.CompareTag("TheBook"))
        {
            Debug.Log("Book entered the area.");

            if (wall != null)
            {
                Destroy(wall);
                Debug.Log("Wall destroyed on server.");
            }
        }
    }
}