using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletChase : NetworkBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float rotationSpeed = 50f; // Degrees per second for smoother turning

    private void Start()
    {
        // Automatically find the player by tag
        if (player == null)
        {
            GameObject[] playerObj = GameObject.FindGameObjectsWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj[Random.Range(0, playerObj.Length)].transform;
            }
        }
    }

    private void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Rotate smoothly toward the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward in the current direction
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return; // Only the host/server handles despawn
    
        if (collision.CompareTag("Player"))
        {
            NetworkObject netObj = GetComponent<NetworkObject>();
            if (netObj.IsSpawned)
            {
                netObj.Despawn();
            }
        }
    }
}