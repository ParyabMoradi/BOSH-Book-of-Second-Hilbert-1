using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class EnemyShooting : NetworkBehaviour
{
    public GameObject bulletPrefab;    // The bullet prefab to spawn
    public float shootInterval = 2f;   // Time interval between shots
    public float bulletSpeed = 10f;    // Speed at which the bullet moves

    private Transform shootPoint;      // The point from where bullets are spawned

    private void Start()
    {
        // Use the center of the enemy (this transform) as shootPoint
        shootPoint = transform;

        // Start shooting repeatedly
        StartCoroutine(ShootBullets());
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    private IEnumerator ShootBullets()
    {
        while (true)
        {
            // Spawn the bullet
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            // Apply movement if it has a Rigidbody2D
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = shootPoint.up * bulletSpeed;
            }

            // Wait before shooting the next bullet
            yield return new WaitForSeconds(shootInterval);
        }
    }
}