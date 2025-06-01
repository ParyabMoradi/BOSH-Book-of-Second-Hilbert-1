using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BulletChase : NetworkBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float rotationSpeed = 50f;

    private void Start()
    {
        if (!IsServer) return; // Bullet logic only runs on server

        FindValidPlayerTarget();
        StartCoroutine(DestroyAfterSeconds(3f)); // Despawn after 4 seconds
    }

    private void Update()
    {
        if (!IsServer) return;

        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindValidPlayerTarget();
            return;
        }

        ChasePlayer();
    }

    private void FindValidPlayerTarget()
    {
        GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");

        if (playerObjs.Length > 0)
        {
            Transform selected = playerObjs[Random.Range(0, playerObjs.Length)].transform;
            if (selected.GetComponent<PlayerHealth>() != null)
            {
                player = selected;
                Debug.Log($"[Bullet] Targeting player: {player.name}");
            }
        }
        else
        {
            Debug.LogWarning("[Bullet] No players found to target.");
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        Debug.Log($"[Bullet] Triggered with: {collision.name}");

        if (collision.CompareTag("Player"))
        {
            var health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log("[Bullet] Hitting player and calling TakeDamage");
                health.TakeDamageServerRpc(1);
            }

            NetworkObject netObj = GetComponent<NetworkObject>();
            if (netObj.IsSpawned)
            {
                netObj.Despawn();
            }
        }
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (IsServer)
        {
            NetworkObject netObj = GetComponent<NetworkObject>();
            if (netObj.IsSpawned)
            {
                netObj.Despawn();
            }
        }
    }
}
