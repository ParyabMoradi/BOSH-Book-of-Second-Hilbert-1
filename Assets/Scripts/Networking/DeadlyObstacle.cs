using UnityEngine;
using Unity.Netcode;

public class DeadlyObstacle : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return; // Only the server applies damage

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamageServerRpc(4);
                Debug.Log($"[Obstacle] Damaged player {health.OwnerClientId}");
            }
        }
    }
}
