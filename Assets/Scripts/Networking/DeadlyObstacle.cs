using UnityEngine;
using Unity.Netcode;

public enum ObstacleType
{
    Deadly,
    OutOfBounds
}

public class DeadlyObstacle : NetworkBehaviour
{
    [SerializeField]
    private ObstacleType obstacleType = ObstacleType.Deadly;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return; // Only the server applies damage or handles out of bounds

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                switch (obstacleType)
                {
                    case ObstacleType.Deadly:
                        health.TakeDamageServerRpc(4);
                        Debug.Log($"[Obstacle] Damaged player {health.OwnerClientId}");
                        break;
                    case ObstacleType.OutOfBounds:
                        health.KillPlayerServerRpc();
                        Debug.Log($"[Obstacle] Player {health.OwnerClientId} out of bounds");
                        break;
                }
            }
        }
    }
}
