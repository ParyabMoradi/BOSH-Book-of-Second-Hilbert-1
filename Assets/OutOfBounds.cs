using UnityEngine;
using Unity.Netcode;

public class OutOfBounds : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!IsServer) return; // Only the owner applies damage

        if (other.CompareTag("Player"))
        {
            Debug.Log("Out of bounds triggered");
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.KillPlayerClientRpc();
                Debug.Log($"[Obstacle] Player {health.OwnerClientId} out of bounds");
            }
        }
    }
}
